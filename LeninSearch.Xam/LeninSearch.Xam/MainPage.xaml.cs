using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.ParagraphAdder;
using Xamarin.Essentials;
using Xamarin.Forms;
using Label = Xamarin.Forms.Label;

namespace LeninSearch.Xam
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly GlobalEvents _globalEvents;
        private IParagraphViewBuilder _paragraphViewBuilder;
        private ParagraphViewBuilderTapSelectionDecorator _selectionSelectionDecorator;
        private readonly CachedLsiProvider _lsiProvider;
        private readonly ICorpusSearch _corpusSearch;
        private readonly ApiService _apiService = new ApiService();
        private readonly IMessage _message = DependencyService.Get<IMessage>();
        private readonly Span _searchResultTitleSpan;
        private bool _isRunningCorpusUpdate = false;

        private State _state;

        public MainPage(GlobalEvents globalEvents)
        {
            _globalEvents = globalEvents;
            InitializeComponent();

            _lsiProvider = new CachedLsiProvider();
            _corpusSearch = new SwitchCorpusSearch(_lsiProvider,
                Settings.OnlineSearch.Host, Settings.OnlineSearch.Port, Settings.OnlineSearch.TimeoutMs,
                Settings.TokenIndexCountCutoff, Settings.ResultCountCutoff);
            //_corpusSearch = new OfflineCorpusSearch(_lsiProvider, Settings.TokenIndexCountCutoff, Settings.ResultCountCutoff);

            // corpus button
            CorpusButton.Pressed += (sender, args) =>
            {
                if (_isRunningCorpusUpdate) return;
                DisplayInitialTabs();
            };

            // updates
            CorpusRefreshButton.Clicked += async (sender, args) => await ShowUpdates();
            Keyboard.Shown += () => CorpusRefreshButton.IsVisible = false;
            Keyboard.Hidden += () => CorpusRefreshButton.IsVisible = true;

            // search entry
            SearchEntry.Text = Settings.Query.Txt2;
            SearchEntry.FontSize = Settings.UI.Font.NormalFontSize;
            SearchEntry.Focused += (sender, args) => { HideTextMenu(); };

            // keyboard
            Keyboard.BindToEntry(SearchEntry);
            Keyboard.SearchClick += async () =>
            {
                if (_isRunningCorpusUpdate) return;
                await OnSearchButtonPressed();
            };
            Keyboard.NonKeyaboardUnfocus += () => CorpusButton.IsVisible = true;

            // paragraphs
            _paragraphViewBuilder = new StdParagraphViewBuilder(_lsiProvider);
            _paragraphViewBuilder = new ParagraphViewBuilderPagerHeaderDecorator(_paragraphViewBuilder);
            _paragraphViewBuilder = new PropertyHolderParagraphViewBuilder(_paragraphViewBuilder, Settings.UI.Font.NormalFontSize);
            _selectionSelectionDecorator = new ParagraphViewBuilderTapSelectionDecorator(_paragraphViewBuilder, _lsiProvider);
            _paragraphViewBuilder = _selectionSelectionDecorator;
            _selectionSelectionDecorator.ParagraphSelectionChanged += async (sender, indexes) =>
            {
                if (indexes.Count > 0)
                {
                    var paragraphIndex = _selectionSelectionDecorator.SelectedIndexes[0];
                    var corpusItem = _state.GetCurrentCorpusItem();
                    var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile);
                    var buttonCount = lsiData.Offsets.ContainsKey(paragraphIndex) ? 3 : 2;
                    await ShowTextMenu(buttonCount);
                }
                else
                {
                    await HideTextMenu();
                }
            };

            // text menu
            BookmarkButton.Clicked += BookmarkButtonOnClicked;
            ShareButton.Clicked += ShareButtonOnClicked;
            HideSearchResultBar();

            // player
            PlayerView.HeightRequest = Settings.UI.BrowserViewHeight;
            PlayerStopButton.Clicked += (sender, args) => StopVideoPlay();
            PlayButton.Clicked += (sender, args) =>
            {
                var paragraphIndex = _selectionSelectionDecorator.SelectedIndexes[0];
                var corpusItem = _state.GetCurrentCorpusItem();
                var corpusFileItem = corpusItem.GetFileByPath(_state.ReadingFile);
                var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile);
                var lsData = lsiData.LsData;
                var headingText = lsData.GetClosestHeadings(paragraphIndex)?.GetText(_lsiProvider.Words(corpusItem.Id));

                PlayerTitleButton.Source = Settings.IconFile(corpusItem.Id);
                PlayerTitleLabel.Text = headingText == null
                    ? corpusFileItem.Name
                    : new string(headingText.Take(30).ToArray());

                var videoId = lsiData.GetVideoId(paragraphIndex);
                var offset = lsiData.Offsets[paragraphIndex];

                StartVideoPlay(videoId, offset);
            };

            _searchResultTitleSpan = AttachCommandToLabel(SearchResultTitle, new Command(async () => await DisplaySearchSummary()));

            PopulateInitialTabs();
        }

        public void CleanCache()
        {
            _lsiProvider.CleanCache();
        }
        public void SetState(State state)
        {
            _state = state;
            var corpusItem = _state.GetCurrentCorpusItem();
            CorpusButton.Source = Settings.IconFile(corpusItem.Id);
            TextMenuStack.WidthRequest = 0;
            SearchActivityIndicator.IsVisible = false;

            if (_state.IsWatchingParagraphSearchResults())
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplaySearchSummary();
                });
            }
            else if (_state.IsReading())
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Read(_state.GetReadingCorpusFileItem(), _state.ReadingParagraphIndex);
                });
            }
            else
            {
                Device.InvokeOnMainThreadAsync(DisplayInitialTabs);
            }
        }

        private void GlobalEventsOnBackButtonPressed(object sender, EventArgs e)
        {
            if (_state.IsReading())
            {
            }
        }

        private async void BookmarkButtonOnClicked(object sender, EventArgs e)
        {
            await Rotate360(BookmarkButton);

            var corpusItem = _state.GetCurrentCorpusItem();
            var corpusFileItem = _state.GetReadingCorpusFileItem();
            if (corpusFileItem == null) return;

            var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, corpusFileItem.Path);
            var pIndex = _selectionSelectionDecorator.SelectedIndexes.First();
            var words = _lsiProvider.GetDictionary(corpusItem.Id).Words;

            var bookmark = new Bookmark
            {
                BookName = corpusFileItem.Name,
                File = corpusFileItem.Path,
                ParagraphIndex = pIndex,
                ParagraphText = lsiData.LsData.Paragraphs[pIndex].GetText(words),
                CorpusItemName = corpusItem.Name,
                CorpusItemId = corpusItem.Id,
                Id = Guid.NewGuid(),
                When = DateTime.UtcNow
            };

            BookmarkRepo.Add(bookmark);
            PopulateBookmarksTab();

            _selectionSelectionDecorator.ClearSelection();
        }

        private async Task ReplaceCorpusWithLoading()
        {
            SearchActivityIndicator.Opacity = 0;
            await CorpusButton.FadeTo(0, 150, Easing.Linear);
            CorpusButton.IsVisible = false;
            SearchActivityIndicator.IsVisible = true;
            SearchActivityIndicator.IsRunning = true;
            await SearchActivityIndicator.FadeTo(1, 150, Easing.Linear);
        }

        private async Task ReplaceLoadingWithCorpus()
        {
            CorpusButton.Opacity = 0;
            await SearchActivityIndicator.FadeTo(0, 150, Easing.Linear);
            SearchActivityIndicator.IsVisible = false;
            CorpusButton.IsVisible = true;
            SearchActivityIndicator.IsRunning = false;
            await CorpusButton.FadeTo(1, 150, Easing.Linear);
        }

        private async void SwipeRight(object sender, EventArgs e)
        {
            if (!_state.CanGoToPrevParagraphSearchResult()) return;

            ResultScroll.IsEnabled = false;
            _state.CurrentParagraphResultIndex--;
            await OnCurrentParagraphResultIndexChange(_state.ReadingFile);
        }

        private async void SwipeLeft(object sender, EventArgs e)
        {
            if (!_state.CanGoToNextParagraphSearchResult()) return;

            ResultScroll.IsEnabled = false;
            _state.CurrentParagraphResultIndex++;
            await OnCurrentParagraphResultIndexChange(_state.ReadingFile);

        }

        private void PopulateCorpusTab()
        {
            CorpusTab.Children.Clear();
            foreach (var ci in State.GetCorpusItems())
            {
                var ciText = ci.ToString();
                var stack = new StackLayout { Orientation = StackOrientation.Horizontal };
                var iconButton = new ImageButton
                {
                    WidthRequest = 32,
                    HeightRequest = 32,
                    BackgroundColor = Color.White,
                    Source = Settings.IconFile(ci.Id),
                    Margin = new Thickness(10, 0, 0, 0)
                };
                iconButton.Clicked += async (sender, args) =>
                {
                    _state.CorpusId = ci.Id;
                    await AnimateDisappear(CorpusButton);
                    CorpusButton.Source = Settings.IconFile(ci.Id);
                    await AnimateAppear(CorpusButton);
                    SearchEntry.GentlyFocus();
                };
                stack.Children.Add(iconButton);
                var textButton = ConstructHyperlinkButton(ciText, Settings.UI.Font.NormalFontSize, async () => await DisplayCorpusBooks(ci));
                stack.Children.Add(textButton);
                CorpusTab.Children.Add(stack);
            }
        }

        private async Task ShowUpdates()
        {
            var summaryResult = await _apiService.GetSummaryAsync(Settings.LsiVersion);
            if (!summaryResult.Success)
            {
                _message.LongAlert(Settings.Misc.ApiError);
                return;
            }

            // 1. calculate updates
            var summary = summaryResult.Summary;
            var updates = new List<CorpusItem>();
            var existingCorpusIds = Settings.GetExistingCorpusIds();
            var existingCorpusItems = existingCorpusIds.Select(id => summary.FirstOrDefault(ci => ci.Id == id))
                .Where(ci => ci != null).ToList();
            foreach (var existingCi in existingCorpusItems)
            {
                var ciSeries = summary.Where(ci => ci.Series == existingCi.Series).OrderByDescending(ci => ci.CorpusVersion).ToList();
                if (ciSeries[0].CorpusVersion > existingCi.CorpusVersion)
                {
                    updates.Add(ciSeries[0]);
                }
            }
            var nonExistingSeries = summary
                .Where(ci => existingCorpusItems.All(eci => eci.Series != ci.Series))
                .OrderByDescending(ci => ci.CorpusVersion)
                .GroupBy(ci => ci.Series)
                .Select(g => g.First());
            updates.AddRange(nonExistingSeries);

            if (updates.Count == 0)
            {
                _message.ShortAlert("Обновлений нет");
                return;
            }

            InitialTabs.IsVisible = false;
            ScrollWrapper.IsVisible = true;
            await RebuildScroll(false);

            var infoLabel = new Label
            {
                Text = "Обновления",
                TextColor = Color.Black,
                FontSize = Settings.UI.Font.SmallFontSize,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };
            ResultStack.Children.Add(infoLabel);

            // 2. display updates list
            foreach (var updateCi in updates)
            {
                View updateLink = null;
                updateLink = ConstructHyperlinkButton(updateCi.ToString(), Settings.UI.Font.NormalFontSize, async () =>
                     {
                         var answer = await DisplayAlert("Установка обновления", $"Установить '{updateCi.Name}'?", "Да", "Нет");

                         if (!answer) return;

                         _isRunningCorpusUpdate = true;
                         await ReplaceCorpusWithLoading();

                         var existingCi = existingCorpusItems.FirstOrDefault(ci => ci.Series == updateCi.Series);
                         if (existingCi != null)
                         {
                             Directory.Delete(Path.Combine(Settings.CorpusRoot, existingCi.Id), true);
                         }

                         var corpusFolder = Path.Combine(Settings.CorpusRoot, updateCi.Id);
                         Directory.CreateDirectory(corpusFolder);

                         foreach (var cfi in updateCi.Files)
                         {
                             infoLabel.IsVisible = true;
                             infoLabel.Text = $"Скачиваю: {cfi.Path}";
                             var cfiBytesResult = await _apiService.GetFileBytesAsync(updateCi.Id, cfi.Path);
                             if (!cfiBytesResult.Success)
                             {
                                 _message.LongAlert(Settings.Misc.ApiError);
                                 Directory.Delete(corpusFolder, true);
                                 infoLabel.Text = "Обновления";
                                 _isRunningCorpusUpdate = false;
                                 await ReplaceLoadingWithCorpus();
                                 return;
                             }
                             await File.WriteAllBytesAsync(Path.Combine(corpusFolder, cfi.Path), cfiBytesResult.Bytes);
                         }

                         _message.LongAlert(Settings.Misc.UpdateCompleteMessage);
                         ResultStack.Children.Remove(updateLink);
                         if (existingCi != null && _state.CorpusId == existingCi.Id)
                         {
                             _state.CorpusId = updateCi.Id;
                             CorpusButton.Source = Settings.IconFile(updateCi.Id);
                         }

                         await ReplaceLoadingWithCorpus();
                         infoLabel.Text = "Обновления";
                         _isRunningCorpusUpdate = false;
                     });
                updateLink.Margin = new Thickness(10, 10, 10, 20);
                ResultStack.Children.Add(updateLink);
            }

            await ResultScrollFadeIn();
        }

        private void PopulateBookmarksTab()
        {
            BookmarkTab.Children.Clear();
            var bookmarks = BookmarkRepo.GetAll().ToList();
            if (bookmarks.Any())
            {
                ResultStack.Children.Add(new Label
                {
                    Text = "ЗАКЛАДКИ",
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.Black,
                    FontSize = Settings.UI.Font.NormalFontSize
                });

                foreach (var bm in bookmarks)
                {
                    var bmText = bm.GetText();

                    var layout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.White,
                        Spacing = 0
                    };

                    var textLabel = new Label
                    {
                        Text = bmText,
                        FontSize = Settings.UI.Font.NormalFontSize,
                        TextColor = Color.Black,
                        Margin = new Thickness(10, 0, 0, 0)
                    };

                    layout.Children.Add(textLabel);

                    var linkLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.White,
                        Spacing = 0
                    };

                    var readLabel = ConstructHyperlinkButton("читать", Settings.UI.Font.NormalFontSize, async () => await ReadBookmark(bm));

                    linkLayout.Children.Add(readLabel);

                    var deleteLabel = ConstructHyperlinkButton("удалить", Settings.UI.Font.NormalFontSize, async () =>
                    {
                        BookmarkRepo.Delete(bm.Id);
                        await layout.FadeTo(0, 200, Easing.Linear);
                        layout.HeightRequest = 0;
                    });

                    linkLayout.Children.Add(deleteLabel);

                    layout.Children.Add(linkLayout);

                    BookmarkTab.Children.Add(layout);
                }
            }
        }

        private async Task ReadBookmark(Bookmark bm)
        {
            try
            {
                var corpusItem = State.GetCorpusItems().FirstOrDefault(ci => ci.Files.Any(cfi => cfi.Path == bm.File));
                if (corpusItem == null) return;
                _state.CorpusId = corpusItem.Id;
                var corpusFileItem = corpusItem.GetFileByPath(bm.File);
                await AnimateDisappear(CorpusButton);
                CorpusButton.Source = Settings.IconFile(corpusItem.Id);
                await AnimateAppear(CorpusButton);
                await Read(corpusFileItem, bm.ParagraphIndex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private void PopulateLearningTab()
        {
            LearningTab.Children.Clear();

            foreach (var video in Settings.Learning)
            {
                var hyperlink = ConstructHyperlinkButton(video.Item1, Settings.UI.Font.NormalFontSize,
                    async () => await Browser.OpenAsync(video.Item2));
                hyperlink.Margin = new Thickness(20, 20, 0, 0);
                LearningTab.Children.Add(hyperlink);
            }
        }

        private void PopulateHistoryTab()
        {
            HistoryTab.Children.Clear();

            var history = HistoryRepo.GetHistory();

            foreach (var historyItem in history)
            {
                var hyperlink = ConstructHyperlinkButton($"{historyItem.Query} ({historyItem.CorpusName})", Settings.UI.Font.NormalFontSize,
                    () =>
                    {
                        _state.CorpusId = historyItem.CorpusId;
                        CorpusButton.Source = Settings.IconFile(historyItem.CorpusId);
                        SearchEntry.Text = historyItem.Query;
                    });
                hyperlink.Margin = new Thickness(20, 20, 0, 0);
                HistoryTab.Children.Add(hyperlink);
            }
        }

        private void PopulateInitialTabs()
        {
            PopulateCorpusTab();
            PopulateBookmarksTab();
            PopulateLearningTab();
            PopulateHistoryTab();
        }

        private void StartVideoPlay(string videoId, ushort offset)
        {
            SearchLayout.IsVisible = false;
            PlayerLayout.IsVisible = true;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            var stream = assembly.GetManifestResourceStream("LeninSearch.Xam.youtube.html");
            var htmlTemplate = "";
            using (var reader = new StreamReader(stream))
            {
                htmlTemplate = reader.ReadToEnd();
            }

            var height = (int)(Settings.UI.BrowserViewHeight / DeviceDisplay.MainDisplayInfo.Density);
            var width = height * 16 / 9;
            var html = htmlTemplate
                .Replace("[videoId]", videoId)
                .Replace("[offset]", offset.ToString())
                .Replace("[width]", width.ToString())
                .Replace("[height]", height.ToString());

            PlayerView.Source = new HtmlWebViewSource { Html = html };
        }

        private void StopVideoPlay()
        {
            SearchLayout.IsVisible = true;
            PlayerLayout.IsVisible = false;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            var stream = assembly.GetManifestResourceStream("LeninSearch.Xam.blank.html");
            var html = "";
            using (var reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }
            PlayerView.Source = new HtmlWebViewSource { Html = html };
        }

        private async Task DisplaySearchSummary()
        {
            if (!_state.IsWatchingParagraphSearchResults()) return;

            HideSearchResultBar();
            StopVideoPlay();
            await RebuildScroll(false);
            var corpusFileItems = _state.PartialParagraphSearchResult.Files().Select(f => _state.GetCurrentCorpusItem().GetFileByPath(f)).ToList();

            var totalCount = _state.PartialParagraphSearchResult.SearchResults.Count;
            var titleView = _state.PartialParagraphSearchResult.IsSearchComplete
                ? (View)new Label { Text = $"Поиск окончен, {totalCount} совпадений", TextColor = Color.Black, FontSize = Settings.UI.Font.SmallFontSize }
                : ConstructHyperlinkButton($"{totalCount} совпадений, нажмите чтобы продолжить", Settings.UI.Font.SmallFontSize, async () =>
                    await StartParagraphSearch(_state.SearchQuery));

            titleView.HorizontalOptions = LayoutOptions.Center;
            titleView.Margin = new Thickness(0, 0, 0, 20);

            ResultStack.Children.Add(titleView);

            var flexLayout = new FlexLayout
            {
                JustifyContent = FlexJustify.SpaceAround,
                Wrap = FlexWrap.Wrap
            };

            ResultStack.Children.Add(flexLayout);

            foreach (var cfi in corpusFileItems)
            {
                var resultCount = _state.PartialParagraphSearchResult.FileResults(cfi.Path).Count;
                var text = $"{cfi.Name} ({resultCount})";
                var link = ConstructHyperlinkButton(text, Settings.UI.Font.NormalFontSize, async () =>
                {
                    _state.ReadingFile = cfi.Path;
                    _state.CurrentParagraphResultIndex = 0;
                    await OnCurrentParagraphResultIndexChange(cfi.Path);
                });
                link.HeightRequest = 32;
                flexLayout.Children.Add(link);
            }

            var fishButton = new ImageButton
            {
                HeightRequest = 32,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.White,
                Source = "fish.png",
                Padding = 0,
                Margin = 0
            };
            ResultStack.Children.Add(fishButton);
            fishButton.Clicked += async (sender, args) => await GenerateSearchFish();

            await ResultScrollFadeIn();
        }

        private async Task GenerateSearchFish()
        {
            await ReplaceCorpusWithLoading();

            var ppsr = _state.PartialParagraphSearchResult;
            var corpusItem = _state.GetCurrentCorpusItem();
            var query = SearchEntry.Text;
            var fishFile = FishGenerator.GenerateFishHtmlFile(ppsr, corpusItem, query, _lsiProvider);

            await ReplaceLoadingWithCorpus();

            await Share.RequestAsync(new ShareFileRequest($"Lenin Search Fish Report - {corpusItem.Name} ({query})", new ShareFile(fishFile)));
        }

        private async void DisplayInitialTabs()
        {
            StopVideoPlay();
            await HideTextMenu();
            PopulateInitialTabs();
            _state.ReadingFile = null;
            HideSearchResultBar();
            ScrollWrapper.IsVisible = false;
            InitialTabs.SelectedIndex = 0;
            InitialTabs.IsVisible = true;
            await RebuildScroll(false);
        }

        private async Task AnimateDisappear(View view)
        {
            await view.FadeTo(0, Settings.UI.DisappearMs, Easing.Linear);
        }

        private async Task AnimateAppear(View view)
        {
            await view.FadeTo(1, Settings.UI.AppearMs, Easing.Linear);
        }

        private async Task Rotate360(View view)
        {
            await view.RotateTo(360, Settings.UI.ButtonRotationMs, Easing.Linear);
            view.Rotation = 0;
        }

        private async void ShareButtonOnClicked(object sender, EventArgs e)
        {
            var corpusItem = _state.GetCurrentCorpusItem();
            if (corpusItem == null) return;
            var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile);
            var indexes = _selectionSelectionDecorator.SelectedIndexes;
            var separator = $"{Environment.NewLine}{Environment.NewLine}";
            var words = _lsiProvider.GetDictionary(corpusItem.Id).Words;

            Func<ushort, string> getParagraphTextFunc = i =>
            {
                var paragraphText = lsiData.LsData.Paragraphs[i].GetText(words);
                if (_state.PartialParagraphSearchResult == null) return paragraphText;
                var searchResult = _state.PartialParagraphSearchResult?.SearchResults?.FirstOrDefault(r => r.ParagraphIndex == i);
                if (searchResult != null)
                {
                    var corpusItem = _state.GetCurrentCorpusItem();
                    var corpusFileItem = corpusItem.Files.First(f => f.Path == searchResult.File);
                    var page = lsiData.LsData.GetClosestPage(searchResult.ParagraphIndex);
                    var headings = lsiData.LsData.GetHeadingsDownToZero(searchResult.ParagraphIndex);
                    string additionalParagaph = $"{corpusItem.Name}, {corpusFileItem.Name}";
                    if (page != null)
                    {
                        additionalParagaph = $"{additionalParagaph}, стр. {page}";
                    }

                    if (headings.Any())
                    {
                        var headingText = string.Join(" - ", headings.Select(h => h.GetText(words)));
                        additionalParagaph = $"{additionalParagaph}, {headingText}";
                    }

                    paragraphText = $"{additionalParagaph}{separator}{paragraphText}";
                }

                return paragraphText;
            };

            var pTexts = indexes.Select(i => getParagraphTextFunc(i)).ToList();
            var shareText = string.Join(separator, pTexts);
            shareText = $"{shareText}{separator}Подготовлено при помощи Lenin Search для Android (доступно в Google Play) за считанные секунды";

            _selectionSelectionDecorator.ClearSelection();

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = "Lenin Search Share"
            });
        }

        private void HideSearchResultBar()
        {
            if (SearchResultBar.Height == 0) return;

            var animation = new Animation(f => SearchResultBar.HeightRequest = f, SearchResultBar.Height, 0, Easing.SinInOut);

            animation.Commit(SearchResultBar, "hideSearchResultBar", 200);
        }

        private void ShowSearchResultBar()
        {
            PrevLabel.Text = (_state.CurrentParagraphResultIndex + 1).ToString();
            NextLabel.Text = _state.PartialParagraphSearchResult.FileResults(_state.ReadingFile).Count.ToString();
            _searchResultTitleSpan.Text = _state.GetReadingCorpusFileItem().Name;

            if (SearchResultBar.Height == 24) return;

            var animation = new Animation(f => SearchResultBar.HeightRequest = f, SearchResultBar.Height, 24, Easing.SinInOut);

            animation.Commit(SearchResultBar, "showSearchResultBar", 200);
        }

        private async Task HideTextMenu()
        {
            var menu = TextMenuStack;
            menu.ScaleY = 1;
            await menu.ScaleYTo(0, Settings.UI.TextMenuAnimationMs, Easing.Linear);
            menu.WidthRequest = 0;
        }

        private async Task ShowTextMenu(int buttonCount)
        {
            var menu = TextMenuStack;
            menu.ScaleY = 0;
            menu.WidthRequest = 42 * buttonCount;
            await menu.ScaleYTo(1, Settings.UI.TextMenuAnimationMs, Easing.Linear);
        }

        private bool IsResultScrollReady()
        {
            if (ResultStack.Children.Count == 0) return false;

            var resultStackHeight = GetResultStackHeight();

            return resultStackHeight > ResultScroll.Height * 1.5;
        }

        private double GetResultStackHeight()
        {
            return ResultStack.Measure(ScrollWrapper.Width, ScrollWrapper.Height).Request.Height;
        }

        private async void ResultScrollOnScrolled(object sender, ScrolledEventArgs e)
        {
            if (string.IsNullOrEmpty(_state.ReadingFile)) return;

            if (!ResultScroll.IsEnabled) return;

            double hiddenHeight = 0;
            for (var i = 0; i < ResultStack.Children.Count; i++)
            {
                var child = ResultStack.Children[i];
                hiddenHeight += child.Height;
                if (hiddenHeight > ResultScroll.ScrollY)
                {
                    _state.ReadingParagraphIndex = (ushort)child.TabIndex;
                    break;
                }
            }

            var corpusItem = _state.GetCurrentCorpusItem();
            var scrollingSpace = ResultScroll.ContentSize.Height - ResultScroll.Height - 11; // 10 is a margin
            if (e.ScrollY == 0) // reached top
            {
                var lsData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile).LsData;

                if (ResultStack.Children.Count > Settings.UI.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromTop(lsData)));
                }
                else
                {
                    double scrollToAfter = 0;
                    while (scrollToAfter < Settings.UI.ScreensPulledOnTopScroll * ResultScroll.Height)
                    {
                        var readingIndexMin = (ushort)ResultStack.Children[0].TabIndex;
                        var p = lsData.GetPrevParagraph(readingIndexMin);
                        if (p == null) return;
                        var pView = _paragraphViewBuilder.Build(p, _state, _lsiProvider.Words(corpusItem.Id));
                        ResultStack.Children.Insert(0, pView);
                        scrollToAfter += ResultStack.Children[0].Height;
                        scrollToAfter += ResultStack.Children[0].Margin.Bottom;
                        scrollToAfter += ResultStack.Children[0].Margin.Top;
                    }

                    ResultScroll.IsEnabled = false;
                    await ResultScroll.ScrollToAsync(0, scrollToAfter, false);
                    ResultScroll.IsEnabled = true;
                }
            }
            else if (scrollingSpace <= e.ScrollY) // reached bottom
            {
                var lsData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile).LsData;

                if (ResultStack.Children.Count > Settings.UI.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromBottom(lsData)));
                }
                else
                {
                    double addedHeight = 0;
                    while (addedHeight < Settings.UI.ScreensPulledOnBottomScroll * ResultScroll.Height)
                    {
                        var readingIndexMax = (ushort)ResultStack.Children.Last().TabIndex;
                        var p = lsData.GetNextParagraph(readingIndexMax);
                        if (p == null) return;
                        var pView = _paragraphViewBuilder.Build(p, _state, _lsiProvider.Words(corpusItem.Id));
                        ResultStack.Children.Add(pView);
                        var lastChild = ResultStack.Children.Last();
                        addedHeight += lastChild.Height;
                        addedHeight += lastChild.Margin.Bottom;
                        addedHeight += lastChild.Margin.Top;
                    }
                }
            }
        }

        private async Task RebuildScroll(LsData lsData, ushort index, double scrollToAfter)
        {
            var corpusItem = _state.GetCurrentCorpusItem();

            await RebuildScroll(false);
            var p = lsData.Paragraphs[index];
            while (!IsResultScrollReady())
            {
                if (p == null) break;
                var pView = _paragraphViewBuilder.Build(p, _state, _lsiProvider.Words(corpusItem.Id));
                ResultStack.Children.Add(pView);
                p = lsData.GetNextParagraph(p.Index);
            }
            await ResultScroll.ScrollToAsync(0, scrollToAfter, false);
            ResultScroll.IsEnabled = false;
            await ResultScrollFadeIn();
            await Task.Delay(250);
            ResultScroll.IsEnabled = true;
            ResultScroll.Scrolled += ResultScrollOnScrolled;
        }

        private async Task RebuildScrollFromTop(LsData lsData)
        {
            var corpusItem = _state.GetCurrentCorpusItem();

            var firstChildIndex = (ushort)ResultStack.Children[0].TabIndex;
            var prevP = lsData.GetPrevParagraph(firstChildIndex);
            if (prevP == null) return;
            var prevView = _paragraphViewBuilder.Build(prevP, _state, _lsiProvider.Words(corpusItem.Id));
            ResultStack.Children.Insert(0, prevView);
            var scrollToAfter = ResultStack.Children[0].Height;
            ResultScroll.Scrolled -= ResultScrollOnScrolled;
            ResultScroll.IsEnabled = false;
            await RebuildScroll(lsData, prevP.Index, scrollToAfter);
        }

        private async Task RebuildScrollFromBottom(LsData lsData)
        {
            double heightFromBottom = 0;
            int childIndex;
            for (childIndex = ResultStack.Children.Count - 1; childIndex >= 0; childIndex--)
            {
                var child = ResultStack.Children[childIndex];
                heightFromBottom += child.Height;
                heightFromBottom += child.Margin.Bottom;
                heightFromBottom += child.Margin.Top;
                if (heightFromBottom > ResultScroll.Height) break;
            }

            var scrollY = heightFromBottom - ResultScroll.Height;
            var pIndex = (ushort)ResultStack.Children[childIndex].TabIndex;
            await RebuildScroll(lsData, pIndex, scrollY);
        }

        private async Task DisplayCorpusBooks(CorpusItem corpusItem)
        {
            InitialTabs.IsVisible = false;
            ScrollWrapper.IsVisible = true;

            _state.CorpusId = corpusItem.Id;
            CorpusButton.Source = Settings.IconFile(corpusItem.Id);

            _state.ReadingFile = null;
            HideSearchResultBar();
            await RebuildScroll(false);

            ResultStack.Children.Add(new Label
            {
                Text = $"{corpusItem.Name} - книги",
                TextColor = Color.Black,
                FontSize = Settings.UI.Font.SmallFontSize,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var flexLayout = new FlexLayout
            {
                JustifyContent = FlexJustify.SpaceAround,
                Wrap = FlexWrap.Wrap
            };

            foreach (var cfi in corpusItem.LsiFiles())
            {
                var link = ConstructHyperlinkButton(cfi.Name, Settings.UI.Font.NormalFontSize, async () =>
                {
                    var lsData = _lsiProvider.GetLsiData(corpusItem.Id, cfi.Path).LsData;
                    var headings = lsData.Headings?.ToList();
                    if (headings?.Any() != true)
                    {
                        _message.ShortAlert("Заголовки не найдены");
                    }
                    else
                    {
                        await ReplaceCorpusWithLoading();
                        await DisplayBookHeadings(corpusItem, cfi, headings);
                        await ReplaceLoadingWithCorpus();
                    }
                });
                flexLayout.Children.Add(link);
            }

            ResultStack.Children.Add(flexLayout);

            await ResultScrollFadeIn();
        }

        private async Task ResultScrollFadeIn()
        {
            await ResultScroll.FadeTo(1, Settings.UI.ResultScrollFadeMs, Easing.Linear);
            ResultScroll.IsEnabled = true;
        }

        private async Task ResultScrollFadeOut()
        {
            ResultScroll.IsEnabled = false;
            await ResultScroll.FadeTo(0, Settings.UI.ResultScrollFadeMs, Easing.Linear);
        }

        private async Task Read(CorpusFileItem cfi, ushort paragraphIndex)
        {
            try
            {
                InitialTabs.IsVisible = false;
                ScrollWrapper.IsVisible = true;

                _state.PartialParagraphSearchResult = null;
                _state.ReadingFile = cfi.Path;
                _state.ReadingParagraphIndex = paragraphIndex;
                _selectionSelectionDecorator.ClearSelection();

                HideSearchResultBar();
                await RebuildScroll(true);

                var corpusItem = _state.GetCurrentCorpusItem();
                var lsData = _lsiProvider.GetLsiData(corpusItem.Id, cfi.Path).LsData;
                var paragraph = lsData.GetPrevParagraph(paragraphIndex);
                if (paragraph == null)
                {
                    paragraphIndex = lsData.Paragraphs.Min(p => p.Key);
                    paragraph = lsData.Paragraphs[paragraphIndex];
                }

                while (!IsResultScrollReady())
                {
                    if (paragraph == null) break;

                    var pView = _paragraphViewBuilder.Build(paragraph, _state, _lsiProvider.Words(corpusItem.Id));

                    ResultStack.Children.Add(pView);

                    paragraph = lsData.GetNextParagraph(paragraph.Index);
                }

                await ResultScroll.ScrollToAsync(0, 20, true);

                await ResultScrollFadeIn();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private async Task DisplayBookHeadings(CorpusItem ci, CorpusFileItem cfi, List<LsHeading> headings)
        {
            _state.ReadingFile = null;
            HideSearchResultBar();
            await RebuildScroll(false);

            var summaryTitle = $"{ci.Name} - {cfi.Name} - содержание";
            ResultStack.Children.Add(new Label
            {
                Text = summaryTitle,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = Settings.UI.Font.SmallFontSize,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var corpusItem = _state.GetCurrentCorpusItem();

            foreach (var heading in headings)
            {
                var headingText = heading.GetText(_lsiProvider.GetDictionary(corpusItem.Id).Words);

                // === hack to align summary to the left ===
                var optimalHeadingLength = Settings.UI.SummaryLineLength;
                if (headingText.Length < optimalHeadingLength)
                {
                    headingText = $"{headingText}{new string(Enumerable.Repeat(' ', optimalHeadingLength - headingText.Length).ToArray())}";
                }
                else if (headingText.Length > optimalHeadingLength)
                {
                    headingText = headingText.Substring(0, optimalHeadingLength);
                }
                // =========================================

                var headingLink = ConstructHyperlinkButton(headingText, Settings.UI.Font.SmallFontSize, async () =>
                    await Read(cfi, heading.Index));

                headingLink.HorizontalOptions = LayoutOptions.Start;
                headingLink.Margin = new Thickness(5 + 8 * heading.Level, 5, 5, 5);
                headingLink.HeightRequest = 18;

                ResultStack.Children.Add(headingLink);
            }

            await ResultScrollFadeIn();
        }

        private async Task DisplaySearchHeadings(List<ParagraphSearchResult> searchResults)
        {
            _state.ReadingFile = null;

            HideSearchResultBar();
            StopVideoPlay();

            foreach (var sr in searchResults)
            {
                var corpusItem = _state.GetCurrentCorpusItem();
                var corpusItemFile = corpusItem.Files.First(cfi => cfi.Path == sr.File);

                var headingText = $"{corpusItem.Name}, {corpusItemFile.Name}, {sr.Text}";
                var headingLabel = new Label
                { Text = headingText, TextColor = Color.Black, FontSize = Settings.UI.Font.NormalFontSize };
                ResultStack.Children.Add(headingLabel);
                var readLink = ConstructHyperlinkButton("читать", Settings.UI.Font.NormalFontSize, async () =>
                    await Read(corpusItemFile, sr.ParagraphIndex));
                ResultStack.Children.Add(readLink);
            }

            await ResultScrollFadeIn();
        }

        private Button ConstructHyperlinkButton(string text, double fontSize, Action action)
        {
            var button = new Button
            {
                BackgroundColor = Color.White,
                TextColor = Settings.UI.MainColor,
                Text = text,
                FontSize = fontSize,
                Padding = 0,
                Margin = 5,
                TextTransform = TextTransform.None,
                HeightRequest = 24,
                MinimumWidthRequest = 48
            };
            button.Clicked += (sender, args) => action();

            return button;

            //var gestureRecognizer = new TapGestureRecognizer { Command = command };
            //var cfiSpan = new Span
            //{
            //    Text = text,
            //    TextColor = Settings.UI.MainColor,
            //    TextDecorations = TextDecorations.Underline,
            //    FontSize = fontSize
            //};
            //cfiSpan.GestureRecognizers.Add(gestureRecognizer);
            //var fString = new FormattedString();
            //fString.Spans.Add(cfiSpan);
            //return new Label { FormattedText = fString, Margin = 5 };
        }

        private Span AttachCommandToLabel(Label label, Command command)
        {
            var gestureRecognizer = new TapGestureRecognizer { Command = command };
            var cfiSpan = new Span
            {
                Text = label.Text,
                TextColor = label.TextColor,
                TextDecorations = label.TextDecorations,
                FontSize = label.FontSize
            };
            cfiSpan.GestureRecognizers.Add(gestureRecognizer);
            var fString = new FormattedString();
            fString.Spans.Add(cfiSpan);

            label.FormattedText = fString;
            label.Text = null;

            return cfiSpan;
        }

        private async Task OnSearchButtonPressed()
        {
            if (string.IsNullOrWhiteSpace(SearchEntry.Text)) return;

            // 1. Initial stuff
            StopVideoPlay();
            InitialTabs.IsVisible = false;
            ScrollWrapper.IsVisible = true;
            _state.SearchQuery = SearchEntry.Text;
            _state.PartialParagraphSearchResult = null;

            // 2. Save history
            var corpusItem = _state.GetCurrentCorpusItem();
            var historyItem = new HistoryItem
            {
                CorpusName = corpusItem.Name,
                CorpusId = corpusItem.Id,
                Query = SearchEntry.Text,
                QueryDateUtc = DateTime.UtcNow
            };
            HistoryRepo.AddHistory(historyItem);
            PopulateHistoryTab();

            // 3. Do search
            var isHeadingSearch = SearchEntry.Text.StartsWith('*');
            if (isHeadingSearch)
            {
                await StartHeadingSearch(SearchEntry.Text);
            }
            else
            {
                await StartParagraphSearch(SearchEntry.Text);
            }
        }

        private async Task StartHeadingSearch(string query)
        {
            await BeforeSearch();

            var corpusItem = _state.GetCurrentCorpusItem();

            var searchResult = await _corpusSearch.SearchAsync(corpusItem.Id, query, null);

            await AfterHeadingSearch(searchResult.SearchResults);
        }

        private async Task StartParagraphSearch(string query)
        {
            if (_state.PartialParagraphSearchResult?.IsSearchComplete == true) return;

            await BeforeSearch();

            var corpusItem = _state.GetCurrentCorpusItem();

            string lastCorpusFile = null;
            if (_state.PartialParagraphSearchResult != null)
            {
                lastCorpusFile = _state.PartialParagraphSearchResult.LastCorpusFile;
            }

            var partialResult = await _corpusSearch.SearchAsync(corpusItem.Id, query, lastCorpusFile);

            _state.PartialParagraphSearchResult ??= new PartialParagraphSearchResult();

            _state.PartialParagraphSearchResult.SearchResults.AddRange(partialResult.SearchResults);
            _state.PartialParagraphSearchResult.LastCorpusFile = partialResult.LastCorpusFile;
            _state.PartialParagraphSearchResult.IsSearchComplete = partialResult.IsSearchComplete;

            await AfterParagraphSearch();
        }

        private async Task BeforeSearch()
        {
            _state.ReadingFile = null;
            HideSearchResultBar();
            await RebuildScroll(false);
            await ReplaceCorpusWithLoading();
            CorpusButton.IsEnabled = false;
        }

        private async Task AfterParagraphSearch()
        {
            await ReplaceLoadingWithCorpus();
            CorpusButton.IsEnabled = true;

            if (_state.PartialParagraphSearchResult.SearchResults.Count == 0)
            {
                ResultStack.Children.Add(new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10),
                    Text = "ничего не найдено",
                    TextColor = Settings.UI.MainColor,
                    FontSize = Settings.UI.Font.NormalFontSize
                });
                await ResultScrollFadeIn();
            }
            else
            {
                await DisplaySearchSummary();
            }
        }

        private async Task AfterHeadingSearch(List<ParagraphSearchResult> searchResults)
        {
            await ReplaceLoadingWithCorpus();
            CorpusButton.IsEnabled = true;

            if (searchResults.Count == 0)
            {
                ResultStack.Children.Add(new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10),
                    Text = "ничего не найдено",
                    TextColor = Settings.UI.MainColor,
                    FontSize = Settings.UI.Font.NormalFontSize
                });
                await ResultScrollFadeIn();
            }
            else
            {
                await DisplaySearchHeadings(searchResults);
            }
        }

        private async Task RebuildScroll(bool attachScrollEvent)
        {
            await ResultScrollFadeOut();

            ScrollWrapper.Children.Clear();
            ResultScroll = new GestureScrollView
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                Opacity = 0,
                IsEnabled = false
            };
            ResultScroll.SwipeLeft += SwipeLeft;
            ResultScroll.SwipeRight += SwipeRight;

            ScrollWrapper.Children.Add(ResultScroll);
            ResultStack = new StackLayout
            {
                Margin = new Thickness(10),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Color.White,
                Spacing = 0
            };
            ResultScroll.Content = ResultStack;
            if (attachScrollEvent)
            {
                ResultScroll.Scrolled += ResultScrollOnScrolled;
            }
        }

        private async Task OnCurrentParagraphResultIndexChange(string file)
        {
            if (!_state.IsWatchingParagraphSearchResults()) return;

            ShowSearchResultBar();

            var corpusItem = _state.GetCurrentCorpusItem();
            var searchResult = _state.GetCurrentSearchParagraphResult();

            _selectionSelectionDecorator.ClearSelection();
            HideTextMenu();
            await RebuildScroll(true);

            var lsData = _lsiProvider.GetLsiData(corpusItem.Id, file).LsData;
            var paragraph = lsData.Paragraphs[searchResult.ParagraphIndex];
            ResultStack.Children.Add(_paragraphViewBuilder.Build(paragraph, _state, _lsiProvider.Words(corpusItem.Id)));
            var prevParagraph = lsData.GetPrevParagraph(paragraph.Index);
            if (prevParagraph != null)
            {
                ResultStack.Children.Insert(0, _paragraphViewBuilder.Build(prevParagraph, _state, _lsiProvider.Words(corpusItem.Id)));
            }
            var maxIndex = paragraph.Index;
            while (!IsResultScrollReady())
            {
                var nextP = lsData.GetNextParagraph(maxIndex);
                if (nextP == null) break;
                maxIndex = nextP.Index;
                ResultStack.Children.Add(_paragraphViewBuilder.Build(nextP, _state, _lsiProvider.Words(corpusItem.Id)));
            }
            await ResultScroll.ScrollToAsync(0, 20, true);
            await ResultScrollFadeIn();
        }
    }
}
