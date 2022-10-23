using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using LeninSearch.Standard.Core.Search.TokenVarying;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.ListItems;
using LeninSearch.Xam.ParagraphAdder;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Label = Xamarin.Forms.Label;

namespace LeninSearch.Xam
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly GlobalEvents _globalEvents;
        private readonly IParagraphViewBuilder _paragraphViewBuilder;
        private readonly ParagraphGestureDecorator _gestureDecorator;
        private readonly TextMenuDecorator _textMenuDecorator;
        private readonly CachedLsiProvider _lsiProvider;
        private readonly ICorpusSearch _corpusSearch;
        private readonly ApiService _apiService = new ApiService();
        private readonly IMessage _message = DependencyService.Get<IMessage>();
        private readonly ITextMeasure _textMeasure = DependencyService.Get<ITextMeasure>();
        private readonly Span _searchResultTitleSpan;
        private bool _isRunningCorpusUpdate = false;

        private State _state;

        public MainPage(GlobalEvents globalEvents)
        {
            _globalEvents = globalEvents;
            InitializeComponent();

            _lsiProvider = new CachedLsiProvider();

            //_corpusSearch = new SwitchCorpusSearch(_lsiProvider,
            //    Settings.Web.Host, Settings.Web.Port, Settings.Web.TimeoutMs,
            //    Settings.TokenIndexCountCutoff, Settings.ResultCountCutoff);

            _corpusSearch = new OfflineCorpusSearch(_lsiProvider, 
                new SearchQueryFactory(new PorterTokenVariantProvider()), 
                Settings.TokenIndexCountCutoff, 
                Settings.ResultCountCutoff);

            // updates
            //CorpusRefreshButton.Clicked += async (sender, args) => await ShowUpdates();

            // search entry
            SearchEntry.Text = "национальный вопрос";
            SearchEntry.FontSize = Settings.UI.Font.NormalFontSize;
            SearchEntry.ReturnCommand = new AsyncCommand(OnSearchButtonPressed);

            // paragraphs
            _paragraphViewBuilder = new StdParagraphViewBuilder(_lsiProvider, () => 100 - 20);
            _gestureDecorator = new ParagraphGestureDecorator(_paragraphViewBuilder);
            var textActions = new TextActions(async (ps, ci, cfi) => await ShareAction(ps, ci, cfi), BookmarkAction, (paragraph, ci, cfi) => {});
            _textMenuDecorator = new TextMenuDecorator(_gestureDecorator, textActions, _lsiProvider);
            _gestureDecorator.ParagraphDoubleTapped += (sender, paragraph) =>
            {
                var autoHide = !_state.IsWatchingSearchResults();
                ShowTextBar(paragraph);
                ShowHeaderStack(autoHide);
            };
            _paragraphViewBuilder = _textMenuDecorator;

            PopulateInitialTabs();
        }

        private void BookmarkAction(LsiParagraph paragraph, CorpusItem ci, CorpusFileItem cfi)
        {
            var words = _lsiProvider.Words(ci.Id);

            var bookmark = new Bookmark
            {
                BookName = cfi.Name,
                File = cfi.Path,
                ParagraphIndex = paragraph.Index,
                ParagraphText = paragraph.GetText(words),
                CorpusItemName = ci.Name,
                CorpusItemId = ci.Id,
                Id = Guid.NewGuid(),
                When = DateTime.UtcNow
            };

            BookmarkRepo.Add(bookmark);
            PopulateBookmarksTab();

            _message.ShortAlert("Закладка добавлена");
        }

        private async Task ShareAction(List<LsiParagraph> paragraphs, CorpusItem ci, CorpusFileItem cfi)
        {
            /*
            var corpusItem = _state.GetCurrentCorpusItem();
            if (corpusItem == null) return;
            var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile);
            var separator = $"{Environment.NewLine}{Environment.NewLine}";
            var words = _lsiProvider.GetDictionary(corpusItem.Id).Words;

            var paragraphTexts = new List<string>();
            foreach (var paragraph in paragraphs)
            {
                var paragraphText = paragraph.GetText(words);

                var searchResult = _state.SearchResult?.SearchResults?.FirstOrDefault(r => r.ParagraphIndex == paragraph.Index);

                if (searchResult == null)
                {
                    paragraphTexts.Add(paragraphText);
                    continue;
                }

                var headings = lsiData.GetHeadingsDownToZero(searchResult.ParagraphIndex);

                paragraphText = headings.Any()
                    ? $"{cfi.Name} - {headings[0].GetText(words)}{separator}{paragraphText}"
                    : $"{cfi.Name}{separator}{paragraphText}";

                paragraphTexts.Add(paragraphText);
            }

            var shareText = string.Join(separator, paragraphTexts);

            shareText = $"{shareText}{separator}Подготовлено при помощи Lenin Search для Android (доступно в Google Play)";

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = "Lenin Search Share"
            });
            */
        }

        public void CleanCache()
        {
            _lsiProvider.CleanCache();
        }


        private CancellationTokenSource _showHeaderCts;
        private void ShowHeaderStack(bool autoHide)
        {
            _showHeaderCts?.Cancel();
            _showHeaderCts = null;
            HeaderStack.IsVisible = true;

            if (autoHide)
            {
                _showHeaderCts = new CancellationTokenSource();
                var ct = _showHeaderCts.Token;
                Task.Run(async () =>
                {
                    await Task.Delay(Settings.UI.HeaderStackAutoHideMs, ct);

                    if (ct.IsCancellationRequested) return;

                    await Device.InvokeOnMainThreadAsync(() => HeaderStack.IsVisible = false);
                }, ct);
            }
        }

        public void SetState(State state)
        {
            _state = state;
            var corpusItem = _state.GetCurrentCorpusItem();
            CorpusTabViewItem.Icon = Settings.IconFile(corpusItem.Id);
            SearchActivityIndicator.IsVisible = false;
        }

        private void GlobalEventsOnBackButtonPressed(object sender, EventArgs e)
        {
            if (_state.IsReading())
            {
            }
        }

        private async Task ReplaceSearchModeButtonWithLoading()
        {
            SearchActivityIndicator.Opacity = 0;
            await SearchModeButton.FadeTo(0, 50, Easing.Linear);
            SearchModeButton.IsVisible = false;
            SearchActivityIndicator.IsVisible = true;
            SearchActivityIndicator.IsRunning = true;
            await SearchActivityIndicator.FadeTo(1, 50, Easing.Linear);
        }

        private async Task ReplaceLoadingWithSearchModeButton()
        {
            SearchModeButton.Opacity = 0;
            await SearchActivityIndicator.FadeTo(0, 50, Easing.Linear);
            SearchActivityIndicator.IsVisible = false;
            SearchModeButton.IsVisible = true;
            SearchActivityIndicator.IsRunning = false;
            await SearchModeButton.FadeTo(1, 50, Easing.Linear);
        }

        private async void SwipeRight(object sender, EventArgs e)
        {
            /*
            if (!_state.CanGoToPrevParagraphSearchResult()) return;

            ResultScroll.IsEnabled = false;
            _state.CurrentSearchUnitIndex--;
            await OnCurrentParagraphResultIndexChange(_state.ReadingFile);
            */
        }

        private async void SwipeLeft(object sender, EventArgs e)
        {
            /*
            if (!_state.CanGoToNextParagraphSearchResult()) return;

            ResultScroll.IsEnabled = false;
            _state.CurrentSearchUnitIndex++;
            await OnCurrentParagraphResultIndexChange(_state.ReadingFile);
            */
        }

        private void RefreshCorpusTab()
        {
            var corpusItems = State.GetCorpusItems().ToList();
            var corpusListItems = corpusItems.Select(CorpusListItem.FromCorpusItem).ToList();
            CorpusListView.ItemsSource = corpusListItems;
        }

        private async Task ShowUpdates()
        {
            /*
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
                var ciStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                ResultStack.Children.Add(ciStack);

                var ciLabel = new Label
                {
                    Text = updateCi.ToString(),
                    TextColor = Color.Black,
                    FontSize = Settings.UI.Font.SmallFontSize,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Start
                };
                ciStack.Children.Add(ciLabel);

                var ciButton = new ImageButton
                {
                    WidthRequest = 24,
                    HeightRequest = 24,
                    BackgroundColor = Color.White,
                    Margin = 0,
                    Padding = 0,
                    Source = "download.png",
                    HorizontalOptions = LayoutOptions.End
                };
                ciStack.Children.Add(ciButton);

                ciButton.Clicked += async (sender, args) =>
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
                    ResultStack.Children.Remove(ciStack);
                    if (existingCi != null && _state.CorpusId == existingCi.Id)
                    {
                        _state.CorpusId = updateCi.Id;
                        CorpusButton.Source = Settings.IconFile(updateCi.Id);
                    }

                    await ReplaceLoadingWithCorpus();
                    infoLabel.Text = "Обновления";
                    _isRunningCorpusUpdate = false;
                };
            }

            await ResultScrollFadeIn();
            */
        }

        private void PopulateBookmarksTab()
        {
            BookmarkTab.Children.Clear();
            var bookmarks = BookmarkRepo.GetAll().ToList();
            if (bookmarks.Any())
            {
                foreach (var bm in bookmarks)
                {
                    var bmText = bm.GetText();

                    var layout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.White,
                        Spacing = 0
                    };

                    var readButton = new ImageButton
                    {
                        Source = "bullet.png",
                        WidthRequest = 32,
                        HeightRequest = 32,
                        BackgroundColor = Color.White,
                        HorizontalOptions = LayoutOptions.Start,
                        Padding = 5,
                        Margin = 0
                    };
                    layout.Children.Add(readButton);

                    var textLabel = new Label
                    {
                        Text = bmText,
                        FontSize = Settings.UI.Font.NormalFontSize,
                        TextColor = Color.Black,
                        Margin = new Thickness(5, 0, 5, 0),
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    layout.Children.Add(textLabel);

                    var trashButton = new ImageButton
                    {
                        Source = "trashred.png",
                        WidthRequest = 32,
                        HeightRequest = 32,
                        BackgroundColor = Color.White,
                        Padding = 5,
                        Margin = 0,
                        HorizontalOptions = LayoutOptions.End
                    };
                    layout.Children.Add(trashButton);

                    readButton.Clicked += async (sender, args) => await ReadBookmark(bm);
                    trashButton.Clicked += async (sender, args) =>
                    {
                        BookmarkRepo.Delete(bm.Id);
                        await layout.FadeTo(0, 200, Easing.Linear);
                        layout.HeightRequest = 0;
                    };

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
                await Read(corpusFileItem, bm.ParagraphIndex);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private void PopulateInitialTabs()
        {
            RefreshCorpusTab();
            PopulateBookmarksTab();
        }

        private async Task DisplaySearchSummary()
        {
            if (!_state.IsWatchingSearchResults()) return;

            /*
            await RebuildScroll(false);
            var corpusFileItems = _state.SearchResult.Files().Select(f => _state.GetCurrentCorpusItem().GetFileByPath(f)).ToList();

            var totalCount = _state.SearchResult.SearchResults.Count;
            var titleView = _state.SearchResult.IsSearchComplete
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
                var resultCount = _state.SearchResult.FileResults(cfi.Path).Count;
                var text = $"{cfi.Name} ({resultCount})";
                var link = ConstructHyperlinkButton(text, Settings.UI.Font.NormalFontSize, async () =>
                {
                    _state.ReadingFile = cfi.Path;
                    _state.CurrentSearchUnitIndex = 0;
                    await OnCurrentParagraphResultIndexChange(cfi.Path);
                });
                link.HeightRequest = 32;
                flexLayout.Children.Add(link);
            }

            var reportButton = new ImageButton
            {
                HeightRequest = 32,
                WidthRequest = 32,
                HorizontalOptions = LayoutOptions.Center,
                BackgroundColor = Color.White,
                Source = "searchreport.png",
                Padding = 0,
                Margin = 0
            };
            ResultStack.Children.Add(reportButton);
            reportButton.Clicked += async (sender, args) => await GenerateSearchReport();

            await ResultScrollFadeIn();
            */
        }

        private async Task GenerateSearchReport()
        {
            var ppsr = _state.SearchResult;
            var corpusItem = _state.GetCurrentCorpusItem();
            var query = SearchEntry.Text;
            var fishFile = SearchReportGenerator.GenerateSearchReportHtmlFile(ppsr, corpusItem, query, _lsiProvider);

            await Share.RequestAsync(new ShareFileRequest($"Lenin Search Report - {corpusItem.Name} ({query})", new ShareFile(fishFile)));
        }

        private async void DisplayInitialTabs()
        {
            PopulateInitialTabs();
            _state.ReadingFile = null;
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

        private void ShowTextBar(LsiParagraph lsiParagraph)
        {
            /*
            var ci = _state.GetCurrentCorpusItem();
            var cfi = _state.GetReadingCorpusFileItem();
            var words = _lsiProvider.Words(ci.Id);
            var lsiData = _lsiProvider.GetLsiData(ci.Id, cfi.Path);
            var headings = lsiData.GetHeadingsDownToZero(lsiParagraph.Index);
            _searchResultTitleSpan.Text = headings.Any()
                ? $"{cfi.Name} - {headings[0].GetText(words)}"
                : cfi.Name;

            if (_state.IsWatchingSearchResults())
            {
                PrevLabel.IsVisible = true;
                PrevLabel.Text = (_state.CurrentSearchUnitIndex + 1).ToString();
                NextLabel.IsVisible = true;
                NextLabel.Text = _state.SearchResult.FileResults(_state.ReadingFile).Count.ToString();
                _searchResultTitleSpan.GestureRecognizers.Clear();
                var tapRecognizer = new TapGestureRecognizer { Command = new Command(async () => await DisplaySearchSummary()) };
                _searchResultTitleSpan.GestureRecognizers.Add(tapRecognizer);
            }
            else
            {
                PrevLabel.IsVisible = false;
                NextLabel.IsVisible = false;
                _searchResultTitleSpan.GestureRecognizers.Clear();
                var tapRecognizer = new TapGestureRecognizer { Command = new Command(async () => await DisplayBookHeadings(ci, cfi, lsiData)) };
                _searchResultTitleSpan.GestureRecognizers.Add(tapRecognizer);
            }

            if (TextBar.Height == 24) return;

            var animation = new Animation(f => TextBar.HeightRequest = f, TextBar.Height, 24, Easing.SinInOut);

            animation.Commit(TextBar, "showTextBar", 200);
            */
        }

        /*
        private bool IsResultScrollReady()
        {
            if (ResultStack.Children.Count == 0) return false;

            var resultStackHeight = GetResultStackHeight();

            return resultStackHeight > ResultScroll.Height * 1.5;
        }
        */

        /*
        private double GetResultStackHeight()
        {
            return ResultStack.Measure(ScrollWrapper.Width, ScrollWrapper.Height).Request.Height;
            
        }
        */

        private async void ResultScrollOnScrolled(object sender, ScrolledEventArgs e)
        {
            /*
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
                var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile);

                if (ResultStack.Children.Count > Settings.UI.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromTop(lsiData)));
                }
                else
                {
                    double scrollToAfter = 0;
                    while (scrollToAfter < Settings.UI.ScreensPulledOnTopScroll * ResultScroll.Height)
                    {
                        var readingIndexMin = (ushort)ResultStack.Children[0].TabIndex;
                        var p = lsiData.GetPrevParagraph(readingIndexMin);
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
                var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, _state.ReadingFile);

                if (ResultStack.Children.Count > Settings.UI.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromBottom(lsiData)));
                }
                else
                {
                    double addedHeight = 0;
                    while (addedHeight < Settings.UI.ScreensPulledOnBottomScroll * ResultScroll.Height)
                    {
                        var readingIndexMax = (ushort)ResultStack.Children.Last().TabIndex;
                        var p = lsiData.GetNextParagraph(readingIndexMax);
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
            */
        }

        private async Task RebuildScroll(LsiData lsiData, ushort index, double scrollToAfter)
        {
            /*
            var corpusItem = _state.GetCurrentCorpusItem();

            await RebuildScroll(false);
            var p = lsiData.Paragraphs[index];
            while (!IsResultScrollReady())
            {
                if (p == null) break;
                var pView = _paragraphViewBuilder.Build(p, _state, _lsiProvider.Words(corpusItem.Id));
                ResultStack.Children.Add(pView);
                p = lsiData.GetNextParagraph(p.Index);
            }
            await ResultScroll.ScrollToAsync(0, scrollToAfter, false);
            ResultScroll.IsEnabled = false;
            await ResultScrollFadeIn();
            await Task.Delay(250);
            ResultScroll.IsEnabled = true;
            ResultScroll.Scrolled += ResultScrollOnScrolled;
            */
        }

        private async Task RebuildScrollFromTop(LsiData lsiData)
        {
            /*
            var corpusItem = _state.GetCurrentCorpusItem();

            var firstChildIndex = (ushort)ResultStack.Children[0].TabIndex;
            var prevP = lsiData.GetPrevParagraph(firstChildIndex);
            if (prevP == null) return;
            var prevView = _paragraphViewBuilder.Build(prevP, _state, _lsiProvider.Words(corpusItem.Id));
            ResultStack.Children.Insert(0, prevView);
            var scrollToAfter = ResultStack.Children[0].Height;
            ResultScroll.Scrolled -= ResultScrollOnScrolled;
            ResultScroll.IsEnabled = false;
            await RebuildScroll(lsiData, prevP.Index, scrollToAfter);
            */
        }

        private async Task RebuildScrollFromBottom(LsiData lsiData)
        {
            /*
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
            await RebuildScroll(lsiData, pIndex, scrollY);
            */
        }

        private async Task DisplayCorpusBooks(CorpusItem corpusItem)
        {
            /*
            InitialTabs.IsVisible = false;
            ScrollWrapper.IsVisible = true;

            _state.CorpusId = corpusItem.Id;
            CorpusButton.Source = Settings.IconFile(corpusItem.Id);

            _state.ReadingFile = null;
            HideTextBar();
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
                    try
                    {
                        var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, cfi.Path);
                        if (lsiData.Headings?.Any() != true)
                        {
                            _message.ShortAlert("Заголовки не найдены");
                        }
                        else
                        {
                            await ReplaceCorpusWithLoading();
                            await DisplayBookHeadings(corpusItem, cfi, lsiData);
                            await ReplaceLoadingWithCorpus();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        throw;
                    }
                    
                });
                flexLayout.Children.Add(link);
            }

            ResultStack.Children.Add(flexLayout);

            await ResultScrollFadeIn();
            */
        }

        private async Task Read(CorpusFileItem cfi, ushort paragraphIndex)
        {
            /*
            try
            {
                InitialTabs.IsVisible = false;
                ScrollWrapper.IsVisible = true;

                _state.SearchResult = null;
                _state.ReadingFile = cfi.Path;
                _state.ReadingParagraphIndex = paragraphIndex;
                _textMenuDecorator.ClearSelection();

                HideTextBar();
                await RebuildScroll(true);

                var corpusItem = _state.GetCurrentCorpusItem();
                var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, cfi.Path);

                var initialParagraph = lsiData.Paragraphs[paragraphIndex];
                var paragraph = initialParagraph;
                while (!IsResultScrollReady())
                {
                    if (paragraph == null) break;

                    var pView = _paragraphViewBuilder.Build(paragraph, _state, _lsiProvider.Words(corpusItem.Id));

                    ResultStack.Children.Add(pView);

                    paragraph = lsiData.GetNextParagraph(paragraph.Index);
                }

                await ResultScroll.ScrollToAsync(0, 20, true);

                await ResultScrollFadeIn();

                ShowTextBar(initialParagraph);

                ShowHeaderStack(true);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
            */
        }

        private async Task DisplayBookHeadings(CorpusItem ci, CorpusFileItem cfi, LsiData lsiData)
        {
            /*
            _state.ReadingFile = null;
            HideTextBar();
            ShowHeaderStack(false);

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

            var dictionaryWords = _lsiProvider.GetDictionary(ci.Id).Words;
            foreach (var headingLeaf in lsiData.HeadingTree.Children)
            {
                var hlView = ViewFactory.ConstructHeadingView(headingLeaf, dictionaryWords, async hl => await Read(cfi, hl.Index));
                ResultStack.Children.Add(hlView);
            }

            await ResultScrollFadeIn();
            */
        }

        private Button ConstructHyperlinkButton(string text, double fontSize, Action action)
        {
            var textWidth = _textMeasure.Width(text, null, (float) fontSize);
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
                WidthRequest = 24 + textWidth
            };
            button.Clicked += (sender, args) => action();

            return button;
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
            SearchEntry.Text = new SearchQueryCleaner().Clean(SearchEntry.Text);
            _state.SearchQuery = SearchEntry.Text;
            _state.SearchResult = null;

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

            await StartSearch(SearchEntry.Text);
        }

        private async Task StartSearch(string query)
        {
            await BeforeSearch();
            var searchMode = GetCurrentSearchMode();
            _state.SearchResult = await _corpusSearch.SearchAsync(_state.CorpusId, query, searchMode);
            await AfterSearch();
        }

        private async Task BeforeSearch()
        {
            _state.ReadingFile = null;
            await ReplaceSearchModeButtonWithLoading();
        }

        private async Task AfterSearch()
        {
            var searchUnitListItems = new List<SearchUnitListItem>();

            foreach (var file in _state.SearchResult.Units.Keys)
            {
                foreach (var query in _state.SearchResult.Units[file].Keys)
                {
                    foreach (var searchUnit in _state.SearchResult.Units[file][query])
                    {
                        searchUnitListItems.Add(new SearchUnitListItem
                        {
                            CorpusId = _state.CorpusId,
                            File = file,
                            SearchUnit = searchUnit
                        });
                    }
                }
            }

            searchUnitListItems = searchUnitListItems.OrderBy(x => x.SearchUnit.Priority).ToList();
            for (ushort i = 0; i < searchUnitListItems.Count; i++)
            {
                searchUnitListItems[i].Index = (ushort)(i + 1);
            }

            SearchResultsView.ItemsSource = searchUnitListItems;

            _message.ShortAlert($"Найдено {searchUnitListItems.Count} совпадений");

            await ReplaceLoadingWithSearchModeButton();
        }

        private async Task RebuildScroll(bool attachScrollEvent)
        {
            /*
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
            */
        }

        private async Task OnCurrentParagraphResultIndexChange(string file)
        {
            /*
            if (!_state.IsWatchingSearchResults()) return;

            var corpusItem = _state.GetCurrentCorpusItem();
            var searchResult = _state.GetCurrentSearchUnit();
            var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, file);
            var paragraph = lsiData.Paragraphs[searchResult.ParagraphIndex];
            ShowTextBar(paragraph);
            _textMenuDecorator.ClearSelection();
            await RebuildScroll(true);

            ResultStack.Children.Add(_paragraphViewBuilder.Build(paragraph, _state, _lsiProvider.Words(corpusItem.Id)));
            var maxIndex = paragraph.Index;
            while (!IsResultScrollReady())
            {
                var nextP = lsiData.GetNextParagraph(maxIndex);
                if (nextP == null) break;
                maxIndex = nextP.Index;
                ResultStack.Children.Add(_paragraphViewBuilder.Build(nextP, _state, _lsiProvider.Words(corpusItem.Id)));
            }
            await ResultScroll.ScrollToAsync(0, 20, true);
            await ResultScrollFadeIn();
            */
        }

        private void TabViewItem_OnTabTapped(object sender, TabTappedEventArgs e)
        {
            Debug.WriteLine("tapped!");
        }

        private void OnCorpusIconClick(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            _state.CorpusId = imageButton.CommandParameter as string;
            CorpusTabViewItem.Icon = Settings.IconFile(_state.CorpusId);
            _message.ShortAlert($"Активирован корпус '{_state.GetCurrentCorpusItem().Name}'");
        }

        private void OnSearchModeButtonClick(object sender, EventArgs e)
        {
            var currentMode = GetCurrentSearchMode();

            if (currentMode == SearchMode.Paragraph)
            {
                SearchModeButton.CommandParameter = ((int)SearchMode.Heading).ToString();
                SearchModeButton.Source = "heading.png";
                _message.ShortAlert("Режим поиска по заголовку");
            }
            else
            {
                SearchModeButton.CommandParameter = ((int)SearchMode.Paragraph).ToString();
                SearchModeButton.Source = "paragraph.png";
                _message.ShortAlert("Режим поиска по тексту");
            }
        }

        private SearchMode GetCurrentSearchMode()
        {
            return (SearchMode)int.Parse(SearchModeButton.CommandParameter as string);
        }
    }
}
