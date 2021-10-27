using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Optimized;
using LeninSearch.Standard.Core.Search;
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
        private ParagraphViewBuilderTapDecorator _selectionDecorator;

        private readonly LsSearcher _searcher = new LsSearcher();

        private Dictionary<string, string> _corpusImages = new Dictionary<string, string>
        {
            { "Ленин ПСС" , "lenin.png" },
            { "Сталин ПСС" , "stalin.png" },
            { "Маркс-Энгельс ПСС" , "marx_engels.png" },
            { "Гегель Наука Логики" , "hegel.png" }
        };

        private State _state;

        public MainPage(GlobalEvents globalEvents)
        {
            _globalEvents = globalEvents;
            InitializeComponent();

            // corpus button
            CorpusButton.Pressed += (sender, args) => DisplayInitialTabs();

            // search entry
            SearchEntry.Text = Settings.Query.Txt2;
            SearchEntry.FontSize = Settings.MainFontSize;
            SearchEntry.Focused += (sender, args) =>
            {
                HideTextMenu();
            };

            // keyboard
            Keyboard.BindToEntry(SearchEntry);
            Keyboard.SearchClick += async () => await OnSearchButtonPressed();
            Keyboard.NonKeyaboardUnfocus += () => CorpusButton.IsVisible = true;

            _paragraphViewBuilder = new StdParagraphViewBuilder();
            _paragraphViewBuilder = new ParagraphViewBuilderPagerHeaderDecorator(_paragraphViewBuilder);
            _paragraphViewBuilder = new PropertyHolderParagraphViewBuilder(_paragraphViewBuilder, Settings.MainFontSize);
            _selectionDecorator = new ParagraphViewBuilderTapDecorator(_paragraphViewBuilder);
            _paragraphViewBuilder = _selectionDecorator;
            _selectionDecorator.ParagraphSelectionChanged += async (sender, indexes) =>
            {
                if (indexes.Count > 0)
                {
                    await ShowTextMenu(_selectionDecorator.SelectedIndexes.Count == 1);
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

            PopulateInitialTabs();
        }
        public void SetState(State state)
        {
            _state = state;
            CorpusButton.Source = _corpusImages[_state.GetCurrentCorpusItem().Name];
            TextMenuStack.WidthRequest = 0;
            SearchActivityIndicator.IsVisible = false;

            if (_state.IsWatchingParagraphSearchResults())
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await OnCurrentParagraphResultIndexChange();
                    ShowSearchResultBar();
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
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayHeaders(_state.GetReadingCorpusFileItem());
                });
            }
        }

        private async void BookmarkButtonOnClicked(object sender, EventArgs e)
        {
            await Rotate360(BookmarkButton);

            var corpusItem = _state.GetCurrentCorpusItem();
            var corpusFileItem = _state.GetReadingCorpusFileItem();
            if (corpusFileItem == null) return;

            var lsiData = LsIndexDataSource.Get(corpusFileItem.Path);
            var pIndex = _selectionDecorator.SelectedIndexes.First();

            var bookmark = new Bookmark
            {
                BookName = corpusFileItem.Name,
                File = corpusFileItem.Path,
                ParagraphIndex = pIndex,
                ParagraphText = lsiData.LsData.Paragraphs[pIndex].GetText(LsDictionary.Instance.Words),
                CorpusItemName = corpusItem.Name,
                Id = Guid.NewGuid(),
                When = DateTime.UtcNow
            };

            BookmarkRepo.Add(bookmark);
            PopulateBookmarksTab();

            _selectionDecorator.ClearSelection();
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
            await OnCurrentParagraphResultIndexChange();
        }

        private async void SwipeLeft(object sender, EventArgs e)
        {
            // 1. continue search if it is not complete
            var isSearchComplete = _state.PartialParagraphSearchResult.IsSearchComplete;
            var isLastSearchResult = _state.CurrentParagraphResultIndex == _state.PartialParagraphSearchResult.SearchResults.Count - 1;
            if (!isSearchComplete && isLastSearchResult)
            {
                await StartParagraphSearch(_state.SearchRequest);
            }
            else
            {
                // 2. go to next search result
                if (!_state.CanGoToNextParagraphSearchResult()) return;
                ResultScroll.IsEnabled = false;
                _state.CurrentParagraphResultIndex++;
                await OnCurrentParagraphResultIndexChange();
            }
        }

        private void PopulateCorpusTab()
        {
            CorpusTab.Children.Clear();
            foreach (var ci in State.CorpusItems)
            {
                var booksString = "книг";
                var fileCountString = ci.Files.Count.ToString();
                if (fileCountString.EndsWith("1")) booksString = "книга";
                if (fileCountString.EndsWith("2") || fileCountString.EndsWith("3") || fileCountString.EndsWith("4")) booksString = "книги";

                var ciText = $"{ci.Name} ({ci.Files.Count} {booksString})";
                var hyperlink = ConstructHyperlink(ciText, new Command(async () =>
                {
                    _state.CorpusName = ci.Name;
                    await AnimateDisappear(CorpusButton);
                    CorpusButton.Source = _corpusImages[ci.Name];
                    await AnimateAppear(CorpusButton);
                    SearchEntry.GentlyFocus();
                    //await DisplayCorpusBooks();
                }), Settings.SummaryFontSize);
                CorpusTab.Children.Add(hyperlink);
            }
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
                    FontSize = Settings.SummaryFontSize
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
                        Text = bmText, FontSize = Settings.SummaryFontSize, TextColor = Color.Black, Margin = new Thickness(10, 0, 0, 0)
                    };

                    layout.Children.Add(textLabel);

                    var linkLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.White,
                        Spacing = 0
                    };

                    var readLabel = ConstructHyperlink("читать", new Command(async () =>
                    {
                        var corpusItem = State.CorpusItems.FirstOrDefault(ci => ci.Files.Any(cfi => cfi.Path == bm.File));

                        if (corpusItem == null) return;
                        
                        var corpusFileItem = corpusItem.GetFileByPath(bm.File);

                        await AnimateDisappear(CorpusButton);
                        CorpusButton.Source = _corpusImages[corpusItem.Name];
                        await AnimateAppear(CorpusButton);

                        await Read(corpusFileItem, bm.ParagraphIndex);

                    }), Settings.SummaryFontSize);

                    linkLayout.Children.Add(readLabel);

                    var deleteLabel = ConstructHyperlink("удалить", new Command(async () =>
                    {
                        BookmarkRepo.Delete(bm.Id);
                        await layout.FadeTo(0, 200, Easing.Linear);
                        layout.HeightRequest = 0;
                    }), Settings.SummaryFontSize);

                    linkLayout.Children.Add(deleteLabel);

                    layout.Children.Add(linkLayout);

                    BookmarkTab.Children.Add(layout);
                }
            }
        }

        private void PopulateLearningTab()
        {
            LearningTab.Children.Clear();

            var searchHyperlink = ConstructHyperlink("КАК РАБОТАТЬ С ПОИСКОВЫМ ЗАПРОСОМ",
                new Command(async () => await Browser.OpenAsync("https://youtu.be/gcCWzO8UwNI")),
                Settings.SummaryFontSize);
            searchHyperlink.Margin = new Thickness(20, 20, 0, 0);
            LearningTab.Children.Add(searchHyperlink);

            var bookmarkHyperlink = ConstructHyperlink("КАК РАБОТАТЬ С ЗАКЛАДКАМИ", 
                new Command(async () => await Browser.OpenAsync("https://youtu.be/p018-wq1wlI")), 
                Settings.SummaryFontSize);
            bookmarkHyperlink.Margin = new Thickness(20, 20, 0, 0);
            LearningTab.Children.Add(bookmarkHyperlink);

            var headingSearchHyperlink = ConstructHyperlink("КАК ИСКАТЬ ПО ЗАГОЛОВКАМ",
                new Command(async () => await Browser.OpenAsync("https://youtu.be/sSy70Vf4TLc")),
                Settings.SummaryFontSize);
            headingSearchHyperlink.Margin = new Thickness(20, 20, 0, 0);
            LearningTab.Children.Add(headingSearchHyperlink);
        }

        private void PopulateInitialTabs()
        {
            PopulateCorpusTab();
            PopulateBookmarksTab();
            PopulateLearningTab();
        }

        private async void DisplayInitialTabs()
        {
            _state.ReadingFile = null;
            HideSearchResultBar();
            ScrollWrapper.IsVisible = false;
            InitialTabs.SelectedIndex = 0;
            InitialTabs.IsVisible = true;
            await RebuildScroll(false);
        }

        private async Task AnimateDisappear(View view)
        {
            await view.FadeTo(0, Settings.DisappearMs, Easing.Linear);
        }

        private async Task AnimateAppear(View view)
        {
            await view.FadeTo(1, Settings.AppearMs, Easing.Linear);
        }

        private async Task Rotate360(View view)
        {
            await view.RotateTo(360, Settings.ButtonRotationMs, Easing.Linear);
            view.Rotation = 0;
        }

        private async void ShareButtonOnClicked(object sender, EventArgs e)
        {
            var lsiData = LsIndexDataSource.Get(_state.ReadingFile);
            var indexes = _selectionDecorator.SelectedIndexes;
            var separator = $"{Environment.NewLine}{Environment.NewLine}";

            Func<ushort, string> getParagraphTextFunc = i =>
            {
                var paragraphText = lsiData.LsData.Paragraphs[i].GetText(LsDictionary.Instance.Words);
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
                        var headingText = string.Join(" - ", headings.Select(h => h.GetText(LsDictionary.Instance.Words)));
                        additionalParagaph = $"{additionalParagaph}, {headingText}";
                    }

                    paragraphText = $"{additionalParagaph}{separator}{paragraphText}";
                }

                return paragraphText;
            };

            var pTexts = indexes.Select(i => getParagraphTextFunc(i)).ToList();
            var shareText = string.Join(separator, pTexts);
            shareText = $"{shareText}{separator}Подготовлено при помощи Lenin Search для Android (доступно в Google Play) за считанные секунды";

            _selectionDecorator.ClearSelection();

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
            if (SearchResultBar.Height == 22) return;

            var animation = new Animation(f => SearchResultBar.HeightRequest = f, SearchResultBar.Height, 22, Easing.SinInOut);

            animation.Commit(SearchResultBar, "showSearchResultBar", 200);
        }

        private async Task HideTextMenu()
        {
            var menu = TextMenuStack;
            menu.ScaleY = 1;
            await menu.ScaleYTo(0, Settings.TextMenuAnimationMs, Easing.Linear);
            menu.WidthRequest = 0;
        }

        private async Task ShowTextMenu(bool showReportButtons)
        {
            var menu = TextMenuStack;
            menu.ScaleY = 0;
            menu.WidthRequest = showReportButtons ? 84 : 42;
            await menu.ScaleYTo(1, Settings.TextMenuAnimationMs, Easing.Linear);
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

            var scrollingSpace = ResultScroll.ContentSize.Height - ResultScroll.Height - 11; // 10 is a margin
            if (e.ScrollY == 0) // reached top
            {
                var lsData = LsIndexDataSource.Get(_state.ReadingFile).LsData;

                if (ResultStack.Children.Count > Settings.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromTop(lsData)));
                }
                else
                {
                    double scrollToAfter = 0;
                    while (scrollToAfter < Settings.ScreensPulledOnTopScroll * ResultScroll.Height)
                    {
                        var readingIndexMin = (ushort)ResultStack.Children[0].TabIndex;
                        var p = lsData.GetPrevParagraph(readingIndexMin);
                        if (p == null) return;
                        var pView = _paragraphViewBuilder.Build(p, _state);
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
                var lsData = LsIndexDataSource.Get(_state.ReadingFile).LsData;
                if (ResultStack.Children.Count > Settings.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromBottom(lsData)));
                }
                else
                {
                    double addedHeight = 0;
                    while (addedHeight < Settings.ScreensPulledOnBottomScroll * ResultScroll.Height)
                    {
                        var readingIndexMax = (ushort)ResultStack.Children.Last().TabIndex;
                        var p = lsData.GetNextParagraph(readingIndexMax);
                        if (p == null) return;
                        var pView = _paragraphViewBuilder.Build(p, _state);
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
            await RebuildScroll(false);
            var p = lsData.Paragraphs[index];
            while (!IsResultScrollReady())
            {
                if (p == null) break;
                var pView = _paragraphViewBuilder.Build(p, _state);
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
            var firstChildIndex = (ushort)ResultStack.Children[0].TabIndex;
            var prevP = lsData.GetPrevParagraph(firstChildIndex);
            if (prevP == null) return;
            var prevView = _paragraphViewBuilder.Build(prevP, _state);
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

        private async Task DisplayCorpusBooks()
        {
            _state.ReadingFile = null;

            HideSearchResultBar();

            await RebuildScroll(false);

            var currentCorpusItem = _state.GetCurrentCorpusItem();

            ResultStack.Children.Add(new Label
            {
                Text = $"{currentCorpusItem.Name} - КНИГИ",
                TextColor = Color.Black,
                FontSize = Settings.SummaryFontSize,
                HorizontalOptions = LayoutOptions.Center
            });

            var flex = new FlexLayout
            {
                Direction = FlexDirection.Row,
                Wrap = FlexWrap.Wrap,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            foreach (var cfi in currentCorpusItem.Files)
            {
                var cfiLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 0,
                    Margin = new Thickness(0, 10, 10, 0)
                };

                // book text
                cfiLayout.Children.Add(new Label
                {
                    Text = cfi.Name,
                    TextColor = Color.Black,
                    FontSize = Settings.SummaryFontSize,
                    WidthRequest = 64,
                    Margin = new Thickness(5)
                });

                // read link
                cfiLayout.Children.Add(ConstructHyperlink("читать", new Command(async () => await Read(cfi, 0)), Settings.SummaryFontSize));

                // summary link
                cfiLayout.Children.Add(ConstructHyperlink("содерж.", new Command(async () => await DisplayHeaders(cfi)), Settings.SummaryFontSize));

                flex.Children.Add(cfiLayout);
            }

            ResultStack.Children.Add(flex);

            await ResultScrollFadeIn();
        }

        private async Task ResultScrollFadeIn()
        {
            await ResultScroll.FadeTo(1, 150, Easing.Linear);
            ResultScroll.IsEnabled = true;
        }

        private async Task ResultScrollFadeOut()
        {
            ResultScroll.IsEnabled = false;
            await ResultScroll.FadeTo(0, 150, Easing.Linear);
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
                _selectionDecorator.ClearSelection();

                HideSearchResultBar();
                await RebuildScroll(true);

                var lsData = LsIndexDataSource.Get(cfi.Path).LsData;
                var paragraph = lsData.GetPrevParagraph(paragraphIndex);
                if (paragraph == null)
                {
                    paragraphIndex = lsData.Paragraphs.Min(p => p.Key);
                    paragraph = lsData.Paragraphs[paragraphIndex];
                }

                while (!IsResultScrollReady())
                {
                    if (paragraph == null) break;

                    var pView = _paragraphViewBuilder.Build(paragraph, _state);

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

        private async Task DisplayHeaders(CorpusFileItem cfi)
        {
            _state.ReadingFile = null;

            HideSearchResultBar();

            await RebuildScroll(false);

            var summaryTitle = $"{cfi.Name} - СОДЕРЖАНИЕ";
            ResultStack.Children.Add(new Label
            {
                Text = summaryTitle,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = Settings.SummaryFontSize
            });

            var lsData = LsIndexDataSource.Get(cfi.Path).LsData;
            var headings = lsData.Headings.ToList();
            if (headings?.Any() != true)
            {
                ResultStack.Children.Add(new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10),
                    Text = "работы не найдены",
                    TextColor = Settings.MainColor,
                    FontSize = Settings.MainFontSize
                });
            }
            else
            {
                foreach (var heading in headings)
                {
                    var headerText = heading.GetText(LsDictionary.Instance.Words);
                    var headerLabel = new Label
                    { Text = headerText, TextColor = Color.Black, FontSize = Settings.SummaryFontSize };
                    ResultStack.Children.Add(headerLabel);
                    var readLink = ConstructHyperlink("читать",
                        new Command(async () => await Read(cfi, heading.Index)), Settings.SummaryFontSize);
                    ResultStack.Children.Add(readLink);
                }
            }

            await ResultScrollFadeIn();
        }

        private async Task DisplaySearchHeadings(List<ParagraphSearchResult> searchResults)
        {
            _state.ReadingFile = null;

            HideSearchResultBar();

            foreach (var sr in searchResults)
            {
                var corpusItem = _state.GetCurrentCorpusItem();
                var corpusItemFile = corpusItem.Files.First(cfi => cfi.Path == sr.File);

                var headingText = $"{corpusItem.Name}, {corpusItemFile.Name}, {sr.Text}";
                var headingLabel = new Label
                { Text = headingText, TextColor = Color.Black, FontSize = Settings.SummaryFontSize };
                ResultStack.Children.Add(headingLabel);
                var readLink = ConstructHyperlink("читать",
                    new Command(async () => await Read(corpusItemFile, sr.ParagraphIndex)), Settings.SummaryFontSize);
                ResultStack.Children.Add(readLink);
            }

            await ResultScrollFadeIn();
        }

        private Label ConstructHyperlink(string text, Command command, double fontSize)
        {
            var gestureRecognizer = new TapGestureRecognizer { Command = command };
            var cfiSpan = new Span
            {
                Text = text,
                TextColor = Settings.MainColor,
                TextDecorations = TextDecorations.Underline,
                FontSize = fontSize
            };
            cfiSpan.GestureRecognizers.Add(gestureRecognizer);
            var fString = new FormattedString();
            fString.Spans.Add(cfiSpan);
            return new Label { FormattedText = fString, Margin = 5 };
        }

        private async Task OnSearchButtonPressed()
        {
            if (string.IsNullOrWhiteSpace(SearchEntry.Text)) return;

            InitialTabs.IsVisible = false;
            ScrollWrapper.IsVisible = true;

            var isHeadingSearch = SearchEntry.Text.StartsWith("*");
            var searchText = SearchEntry.Text.TrimStart('*');
            var searchRequest = SearchRequest.Construct(searchText, LsDictionary.Instance.Words);
            _state.SearchRequest = searchRequest;
            _state.PartialParagraphSearchResult = null;

            if (isHeadingSearch)
            {
                await StartHeadingSearch(searchRequest);
            }
            else
            {
                await StartParagraphSearch(searchRequest);
            }
        }

        private async Task StartHeadingSearch(SearchRequest searchRequest)
        {
            await BeforeSearch();

            var corpusItem = _state.GetCurrentCorpusItem();

            var searchResults = new List<ParagraphSearchResult>();

            if (ConcurrentOptions.OneByOne)
            {
                foreach (var fileItem in corpusItem.Files)
                {
                    var results = SearchCorpusFileItem(fileItem, searchRequest, true);
                    searchResults.AddRange(results);
                }
            }
            else
            {
                for (var i = 0; i < corpusItem.Files.Count; i += ConcurrentOptions.LsToLsiBatchSize)
                {
                    var tasks = corpusItem.Files.Skip(i).Take(ConcurrentOptions.LsToLsiBatchSize)
                        .Select(cfi => Task.Run(() => SearchCorpusFileItem(cfi, searchRequest, true)));

                    var resultsList = Task.WhenAll(tasks).Result;

                    foreach (var results in resultsList) searchResults.AddRange(results);
                }
            }

            await AfterHeadingSearch(searchResults);
        }

        private async Task StartParagraphSearch(SearchRequest searchRequest)
        {
            if (_state.PartialParagraphSearchResult?.IsSearchComplete == true) return;

            await BeforeSearch();

            var corpusItem = _state.GetCurrentCorpusItem();
            string lastCorpusFile = null;
            var beforeSearchCount = 0;
            if (_state.PartialParagraphSearchResult != null)
            {
                lastCorpusFile = _state.PartialParagraphSearchResult.LastCorpusFile;
                beforeSearchCount = _state.PartialParagraphSearchResult.SearchResults.Count;
            }

            var partialResult = DoPartialParagraphSearch(corpusItem, lastCorpusFile, searchRequest);

            _state.PartialParagraphSearchResult ??= new PartialParagraphSearchResult();

            _state.PartialParagraphSearchResult.SearchResults.AddRange(partialResult.SearchResults);
            _state.PartialParagraphSearchResult.LastCorpusFile = partialResult.LastCorpusFile;
            _state.PartialParagraphSearchResult.IsSearchComplete = partialResult.IsSearchComplete;

            await AfterParagraphSearch(beforeSearchCount);
        }

        private async Task BeforeSearch()
        {
            _state.ReadingFile = null;
            HideSearchResultBar();
            await RebuildScroll(false);
            await ReplaceCorpusWithLoading();
            CorpusButton.IsEnabled = false;
        }

        private async Task AfterParagraphSearch(int beforeSearchCount)
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
                    TextColor = Settings.MainColor,
                    FontSize = Settings.MainFontSize
                });
                await ResultScrollFadeIn();
            }
            else
            {
                var corpusItem = State.CorpusItems.First(ci => ci.Name == _state.CorpusName);

                SearchResultTitle.Text = "результаты поиска (полные)";
                if (!_state.PartialParagraphSearchResult.IsSearchComplete)
                {
                    var firstCfiName = corpusItem.Files[0].Name;
                    var lastCfiName = corpusItem.Files.First(cfi => cfi.Path == _state.PartialParagraphSearchResult.LastCorpusFile).Name;
                    SearchResultTitle.Text = $"результаты поиска ({firstCfiName} - {lastCfiName})";
                }

                ShowSearchResultBar();
                _state.CurrentParagraphResultIndex = _state.PartialParagraphSearchResult.SearchResults.Count > beforeSearchCount
                    ? beforeSearchCount
                    : beforeSearchCount - 1;
                await OnCurrentParagraphResultIndexChange();
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
                    TextColor = Settings.MainColor,
                    FontSize = Settings.MainFontSize
                });
                await ResultScrollFadeIn();
            }
            else
            {
                await DisplaySearchHeadings(searchResults);
            }
        }

        private PartialParagraphSearchResult DoPartialParagraphSearch(CorpusItem corpusItem, string lastSearchedFile, SearchRequest searchRequest)
        {
            var partialResult = new PartialParagraphSearchResult
            {
                IsSearchComplete = false,
                SearchResults = new List<ParagraphSearchResult>()
            };

            var corpusFileItems = lastSearchedFile == null
                ? corpusItem.Files
                : corpusItem.Files.SkipWhile(cfi => cfi.Path != lastSearchedFile).Skip(1).ToList();

            if (ConcurrentOptions.OneByOne)
            {
                foreach (var fileItem in corpusFileItems)
                {
                    var results = SearchCorpusFileItem(fileItem, searchRequest);
                    if (results.Count > 0)
                    {
                        partialResult.SearchResults.AddRange(results);
                        partialResult.LastCorpusFile = fileItem.Path;
                        return partialResult;
                    }
                }
            }
            else
            {
                for (var i = 0; i < corpusFileItems.Count; i += ConcurrentOptions.LsToLsiBatchSize)
                {
                    var cfiBatch = corpusFileItems.Skip(i).Take(ConcurrentOptions.LsToLsiBatchSize).ToList();
                    var tasks = cfiBatch.Select(cfi => Task.Run(() => SearchCorpusFileItem(cfi, searchRequest)));

                    var results = Task.WhenAll(tasks).Result.SelectMany(r => r).ToList();

                    if (results.Count > 0)
                    {
                        partialResult.SearchResults.AddRange(results);
                        partialResult.LastCorpusFile = cfiBatch.Last().Path;
                        return partialResult;
                    }
                }

                partialResult.LastCorpusFile = corpusFileItems.Last().Path;
            }

            partialResult.IsSearchComplete = true;

            return partialResult;
        }

        private List<ParagraphSearchResult> SearchCorpusFileItem(CorpusFileItem cfi, SearchRequest searchRequest, bool isHeadingSearch = false)
        {
            var lsiData = LsIndexDataSource.Get(cfi.Path);

            var results = isHeadingSearch
                ? _searcher.SearchHeadings(lsiData, searchRequest)
                : _searcher.SearchParagraphs(lsiData, searchRequest);

            foreach (var r in results)
            {
                r.File = cfi.Path;
                if (isHeadingSearch)
                {
                    var heading = lsiData.LsData.Headings.First(h => h.Index == r.ParagraphIndex);
                    r.Text = heading.GetText(LsDictionary.Instance.Words);
                }
            }

            return results;
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

        private async Task OnCurrentParagraphResultIndexChange()
        {
            if (!_state.IsWatchingParagraphSearchResults()) return;
            var paragraphResult = _state.GetCurrentSearchParagraphResult();

            PrevLabel.Text = (_state.CurrentParagraphResultIndex + 1).ToString();
            NextLabel.Text = _state.PartialParagraphSearchResult.SearchResults.Count.ToString();

            _selectionDecorator.ClearSelection();

            HideTextMenu();

            await RebuildScroll(true);

            _state.ReadingFile = paragraphResult.File;
            var lsData = LsIndexDataSource.Get(paragraphResult.File).LsData;

            var paragraph = lsData.Paragraphs[paragraphResult.ParagraphIndex];

            ResultStack.Children.Add(_paragraphViewBuilder.Build(paragraph, _state));
            var prevParagraph = lsData.GetPrevParagraph(paragraph.Index);
            ResultStack.Children.Insert(0, _paragraphViewBuilder.Build(prevParagraph, _state));
            var maxIndex = paragraph.Index;
            while (!IsResultScrollReady())
            {
                var nextP = lsData.GetNextParagraph(maxIndex);
                if (nextP == null) break;
                maxIndex = nextP.Index;
                ResultStack.Children.Add(_paragraphViewBuilder.Build(nextP, _state));
            }

            await ResultScroll.ScrollToAsync(0, 20, true);

            await ResultScrollFadeIn();
        }
    }
}
