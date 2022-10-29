using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using LeninSearch.Standard.Core.Search.TokenVarying;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.ListItems;
using LeninSearch.Xam.ParagraphAdder;
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
            SearchEntry.ReturnCommand = new Command(OnSearchButtonPressed);

            // paragraphs
            PopulateInitialTabs();
        }

        private void ActivateCorpus(string corpusId)
        {
            var corpusItem = _lsiProvider.GetCorpusItem(corpusId);
            var summaryBookListItems = SummaryBookListItem.Construct(corpusItem).ToList();
            CorpusTabViewItem.Icon = Settings.IconFile(corpusId);
            SummaryBookPicker.ItemsSource = summaryBookListItems;
            SummaryBookPicker.SelectedIndex = 0;
            _state.CorpusId = corpusId;
            _message.ShortAlert($"Активирован корпус '{corpusItem.Name}'");
        }

        public void CleanCache()
        {
            _lsiProvider.CleanCache();
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

        private void ReplaceSearchModeButtonWithLoading()
        {
            SearchModeButton.IsVisible = false;
            SearchActivityIndicator.IsVisible = true;
            SearchActivityIndicator.IsRunning = true;
        }

        private void ReplaceLoadingWithSearchModeButton()
        {
            SearchActivityIndicator.IsVisible = false;
            SearchModeButton.IsVisible = true;
            SearchActivityIndicator.IsRunning = false;
        }

        private void RefreshCorpusTab()
        {
            var corpusItems = State.GetCorpusItems().ToList();
            var corpusListItems = corpusItems.Select(CorpusListItem.FromCorpusItem).ToList();
            CorpusCollectionView.ItemsSource = corpusListItems;
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

        private void RefreshBookmarksTab()
        {
            var bookmarkListItems = BookmarkRepo.GetAll()
                .OrderByDescending(b => b.When)
                .Select(BookmarkListItem.FromBookmark).ToList();
            BookmarkCollectionView.ItemsSource = bookmarkListItems;
        }

        private void PopulateInitialTabs()
        {
            RefreshCorpusTab();
            RefreshBookmarksTab();
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

        private void OnSearchButtonPressed()
        {
            if (string.IsNullOrWhiteSpace(SearchEntry.Text)) return;

            // 1. Initial stuff
            SearchEntry.Text = new SearchQueryCleaner().Clean(SearchEntry.Text);
            _state.SearchQuery = SearchEntry.Text;
            _state.SearchResult = null;
            _state.ReadingFile = null;
            ReplaceSearchModeButtonWithLoading();

            Task.Run(async () =>
            {
                // 1. Save history
                var corpusItem = _state.GetCurrentCorpusItem();
                var historyItem = new HistoryItem
                {
                    CorpusName = corpusItem.Name,
                    CorpusId = corpusItem.Id,
                    Query = SearchEntry.Text,
                    QueryDateUtc = DateTime.UtcNow
                };
                HistoryRepo.AddHistory(historyItem);

                var searchMode = GetCurrentSearchMode();
                _state.SearchResult = await _corpusSearch.SearchAsync(_state.CorpusId, SearchEntry.Text, searchMode);

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
                                SearchUnit = searchUnit,
                                Query = query
                            });
                        }
                    }
                }

                searchUnitListItems = searchUnitListItems.OrderBy(x => x.SearchUnit.Priority).ToList();
                for (ushort i = 0; i < searchUnitListItems.Count; i++)
                {
                    searchUnitListItems[i].Index = (ushort)(i + 1);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    SearchResultsView.ItemsSource = searchUnitListItems;
                    _message.ShortAlert($"Найдено {searchUnitListItems.Count} совпадений");
                    ReplaceLoadingWithSearchModeButton();
                });
            });
        }

        private void OnCorpusIconClick(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            Animations.OpacityToZeroAndBack(imageButton);
            ActivateCorpus(imageButton.CommandParameter as string);
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

        private void OnSearchUnitTapped(object sender, EventArgs e)
        {
            var stackLayout = (sender as StackLayout);
            Animations.OpacityToZeroAndBack(stackLayout);
            var gestureRecognizer = stackLayout.GestureRecognizers[0] as TapGestureRecognizer;
            var searchUnitListItem = gestureRecognizer.CommandParameter as SearchUnitListItem;
            RunReadBook(searchUnitListItem.CorpusId, searchUnitListItem.File, searchUnitListItem.SearchUnit.ParagraphIndex, searchUnitListItem.SearchUnit);
        }

        private void SummaryBookPickerOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var summaryBookListItem = SummaryBookPicker.SelectedItem as SummaryBookListItem;

            if (summaryBookListItem == null) return;

            var lsiData = _lsiProvider.GetLsiData(summaryBookListItem.CorpusId, summaryBookListItem.File);
            var dictionary = _lsiProvider.GetDictionary(summaryBookListItem.CorpusId);
            var summaryListItems = lsiData.HeadingParagraphs.OrderBy(h => h.Index).Select(hp => new SummaryListItem
            {
                CorpusId = summaryBookListItem.CorpusId,
                File = summaryBookListItem.File,
                ParagraphIndex = hp.Index,
                Title = hp.GetText(dictionary.Words),
                Padding = new Thickness(10 + 20 * hp.HeadingLevel, 0, 10, 0)
            });
            SummaryView.ItemsSource = summaryListItems;
        }

        private void OnSummaryItemTapped(object sender, EventArgs e)
        {
            var stackLayout = sender as StackLayout;
            var summaryListItem = (stackLayout.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as SummaryListItem;
            RunReadBook(summaryListItem.CorpusId, summaryListItem.File, summaryListItem.ParagraphIndex);
        }

        private void OnReadItemTapped(object sender, EventArgs e)
        {
            var stackLayout = sender as StackLayout;
            var gestureRecognizer = stackLayout.GestureRecognizers[0] as TapGestureRecognizer;
            var readListItem = gestureRecognizer.CommandParameter as ReadListItem;
            if (readListItem.Info == null)
            {
                var lsiData = _lsiProvider.GetLsiData(readListItem.CorpusId, readListItem.File);
                var dictionary = _lsiProvider.GetDictionary(readListItem.CorpusId);
                var headingChain = lsiData.GetHeadingsDownToZero(readListItem.ParagraphIndex);
                var headingTexts = headingChain.Select(h => h.GetText(dictionary.Words));
                readListItem.Info = string.Join(" - ", headingTexts);
            }
            readListItem.IsMenuShown = !readListItem.IsMenuShown;
        }

        private void OnReadImageTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var gestureRecognizer = image.GestureRecognizers[0] as TapGestureRecognizer;
            var readListItem = gestureRecognizer.CommandParameter as ReadListItem;
            readListItem.ImageZoomCoefficient = readListItem.ImageZoomCoefficient > 1 ? 1 : (float)1.5;

            // hack: disable swipe when image is zoomed
            MainTabs.IsSwipeEnabled = readListItem.ImageZoomCoefficient == 1;
        }

        private void OnMainTabsSelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            // hack: restore swipe after image zoom
            MainTabs.IsSwipeEnabled = true;
        }

        private void OnAddBookmarkClick(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var readListItem = imageButton.CommandParameter as ReadListItem;
            var dictionary = _lsiProvider.GetDictionary(readListItem.CorpusId);
            var corpusItem = _lsiProvider.GetCorpusItem(readListItem.CorpusId);
            var corpusFileItem = corpusItem.GetFileByPath(readListItem.File);
            var lsiData = _lsiProvider.GetLsiData(readListItem.CorpusId, readListItem.File);
            var lsiParagraph = lsiData.Paragraphs[readListItem.ParagraphIndex];

            var bookmark = new Bookmark
            {
                BookName = corpusFileItem.Name,
                File = corpusFileItem.Path,
                ParagraphIndex = readListItem.ParagraphIndex,
                ParagraphText = lsiParagraph.GetText(dictionary.Words),
                CorpusItemName = corpusItem.Name,
                CorpusItemId = corpusItem.Id,
                Id = Guid.NewGuid(),
                When = DateTime.UtcNow
            };

            BookmarkRepo.Add(bookmark);
            RefreshBookmarksTab();

            _message.ShortAlert("Закладка добавлена");
        }

        private void RunReadBook(string corpusId, string file, ushort selectedParagraphIndex, SearchUnit searchUnit = null)
        {
            Task.Run(() =>
            {
                var lsiData = _lsiProvider.GetLsiData(corpusId, file);
                var corpusFileItem = _lsiProvider.GetCorpusItem(corpusId)
                    .GetFileByPath(file);
                var readListItems = new List<ReadListItem>();
                ReadListItem scrollTo = null;
                foreach (var paragraphIndex in lsiData.Paragraphs.Keys.OrderBy(k => k))
                {
                    var lsiParagraph = lsiData.Paragraphs[paragraphIndex];
                    var readListItem = ReadListItem.Construct(corpusId, file,
                        lsiParagraph, _lsiProvider, searchUnit, () => (ushort)MainTabs.Width,
                        async s => await DisplayAlert("", s, "OK", FlowDirection.MatchParent));
                    readListItems.Add(readListItem);
                    if (paragraphIndex == selectedParagraphIndex)
                    {
                        scrollTo = readListItem;
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    ReadBookTitleLabel.Text = corpusFileItem.Name;
                    ReadView.ItemsSource = readListItems;
                    MainTabs.SelectedIndex = MainTabs.TabItems.IndexOf(ReadTabViewItem);
                    ReadView.ScrollTo(scrollTo, null, ScrollToPosition.MakeVisible, false);
                });
            });
        }

        private void OnOpenBookmarkClick(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var bookmarkListItem = imageButton.CommandParameter as BookmarkListItem;
            Animations.OpacityToZeroAndBack(imageButton);
            ActivateCorpus(bookmarkListItem.CorpusId);
            RunReadBook(bookmarkListItem.CorpusId, bookmarkListItem.File, bookmarkListItem.ParagraphIndex);
        }

        private void OnDeleteBookmarkClick(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var bookmarkListItem = imageButton.CommandParameter as BookmarkListItem;
            BookmarkRepo.Delete(bookmarkListItem.BookmarkId);
            RefreshBookmarksTab();
            _message.ShortAlert("Закладка удалена");
        }

        private void OnShareClicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var readListItem = imageButton.CommandParameter as ReadListItem;

            var corpusItem = _lsiProvider.GetCorpusItem(readListItem.CorpusId);
            var corpusFileItem = corpusItem.GetFileByPath(readListItem.File);
            var lsiData = _lsiProvider.GetLsiData(readListItem.CorpusId, readListItem.File);
            var separator = $"{Environment.NewLine}{Environment.NewLine}";
            var words = _lsiProvider.GetDictionary(corpusItem.Id).Words;
            var lsiParagraph = lsiData.Paragraphs[readListItem.ParagraphIndex];

            var shareText = lsiParagraph.GetText(words);

            var headings = lsiData.GetHeadingsDownToZero(readListItem.ParagraphIndex);

            shareText = headings.Any()
                ? $"{corpusFileItem.Name} - {headings[0].GetText(words)}{separator}{shareText}"
                : $"{corpusFileItem.Name}{separator}{shareText}";

            shareText = $"{shareText}{separator}Подготовлено при помощи Lenin Search для Android (доступно в Google Play)";

            Share.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = "Lenin Search Share"
            }).Wait();
        }
    }
}
