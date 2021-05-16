using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Oprimized;
using LeninSearch.Standard.Core.Reporting;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.ParagraphAdder;
using LeninSearch.Xam.Searcher;
using Newtonsoft.Json;
using Plugin.Clipboard;
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

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly ISearcher _searcher = new OfflineSearcher();

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
            CorpusButton.Pressed += (sender, args) => DisplayCorpus();

            SearchEntry.Text = null;
            SearchEntry.FontSize = Settings.MainFontSize;
            SearchEntry.ReturnCommand = new Command(async () => await OnSearchButtonPressed());
            SearchEntry.Focused += (sender, args) => HideTextMenu();

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
                    await HideBugMenu();
                }
            };

            // text menu
            BookmarkButton.Clicked += BookmarkButtonOnClicked;
            ClipboardButton.Clicked += ClipboardButtonOnClicked;
            ExclamationButton.Clicked += ExclamationButtonOnClicked;

            // bug menu
            var reportGestureRecognizer = new TapGestureRecognizer();
            reportGestureRecognizer.Tapped += ReportButtonClick;
            OcrButton.GestureRecognizers.Add(reportGestureRecognizer);
            XheaderButton.GestureRecognizers.Add(reportGestureRecognizer);
            XfootnoteButton.GestureRecognizers.Add(reportGestureRecognizer);

            HideSearchResultBar();
        }

        public void SetState(State state)
        {
            _state = state;
            CorpusButton.Source = _corpusImages[_state.GetCurrentCorpusItem().Name];
            BugPanel.HeightRequest = 0;
            TextMenuStack.WidthRequest = 0;
            SearchActivityIndicator.IsVisible = false;

            if (_state.IsWatchingSearchResults())
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
                Device.InvokeOnMainThreadAsync(DisplayCorpus);
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

            var ofd = OptimizedFileDataSource.Get(corpusFileItem.Path);
            var pIndex = _selectionDecorator.SelectedIndexes.First();

            var bookmark = new Bookmark
            {
                BookName = corpusFileItem.Name,
                File = corpusFileItem.Path,
                ParagraphIndex = pIndex,
                ParagraphText = ofd.GetParagraph(pIndex).GetText(OptimizedDictionary.Instance.Words),
                CorpusItemName = corpusItem.Name,
                Id = Guid.NewGuid(),
                When = DateTime.UtcNow
            };

            BookmarkRepo.Add(bookmark);

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
            if (!_state.CanGoToPrevSearchResult()) return;

            ResultScroll.IsEnabled = false;
            _state.CurrentParagraphResultIndex--;
            await OnCurrentParagraphResultIndexChange();
        }

        private async void SwipeLeft(object sender, EventArgs e)
        {
            if (!_state.CanGoToNextSearchResult()) return;

            ResultScroll.IsEnabled = false;
            _state.CurrentParagraphResultIndex++;
            await OnCurrentParagraphResultIndexChange();
        }

        private async void DisplayCorpus()
        {
            _state.ReadingFile = null;

            HideSearchResultBar();

            await RebuildScroll(false);

            ResultStack.Children.Add(new Label
            {
                Text = "КОРПУС",
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
                FontSize = Settings.SummaryFontSize
            });

            // output corpus
            foreach (var ci in State.CorpusItems)
            {
                var ciText = $"{ci.Name} {ci.Description}";
                var label = ConstructHyperlink(ciText, new Command(async () =>
                {
                    _state.CorpusName = ci.Name;
                    await AnimateDisappear(CorpusButton);
                    CorpusButton.Source = _corpusImages[ci.Name];
                    await AnimateAppear(CorpusButton);
                    await DisplayCorpusBooks();
                }), Settings.SummaryFontSize);
                ResultStack.Children.Add(label);
            }

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

                    layout.Children.Add(new Label { Text = bmText, FontSize = Settings.SummaryFontSize, TextColor = Color.Black });

                    var linkLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.White,
                        Spacing = 0
                    };

                    var readLabel = ConstructHyperlink("читать", new Command(async () =>
                    {
                        var corpusItem = _state.GetCurrentCorpusItem();
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

                    ResultStack.Children.Add(layout);
                }
            }

            await ResultScrollFadeIn();
        }

        private async void ReportButtonClick(object sender, EventArgs args)
        {
            var btn = sender as Image;
            var index = _selectionDecorator.SelectedIndexes.FirstOrDefault();
            if (index == default) return;
            await Rotate360(btn);
            _selectionDecorator.ClearSelection();

            var type = sender == OcrButton
                ? ReportType.Ocr
                : sender == XheaderButton
                    ? ReportType.Xheader
                    : ReportType.Xfootnote;

            var result = await Report(ReportItem.Construct(type, _state.ReadingFile, index));
            if (!result)
            {
                await DisplayAlert($"Отчет {type}", "Не удалось отправить отчет. Проверьте соединение с интеренетом", "OK");
            }
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

        private async Task<bool> Report(ReportItem item)
        {
            try
            {
                var url = "https://lsreporter.azurewebsites.net/api/LsReporter";
                var content = JsonConvert.SerializeObject(item);
                var response = await _httpClient.PostAsync(url, new StringContent(content));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async void ClipboardButtonOnClicked(object sender, EventArgs e)
        {
            await Rotate360(ClipboardButton);
            var ofd = OptimizedFileDataSource.Get(_state.ReadingFile);
            var indexes = _selectionDecorator.SelectedIndexes;
            var pTexts = indexes.Select(i => ofd.GetParagraph(i).GetText(OptimizedDictionary.Instance.Words)).ToList();
            var separator = $"{Environment.NewLine}{Environment.NewLine}";
            var cbText = string.Join(separator, pTexts);
            CrossClipboard.Current.SetText(cbText);
            await AnimateAppear(ClipboardButton);
            _selectionDecorator.ClearSelection();
        }

        private async void ExclamationButtonOnClicked(object sender, EventArgs e)
        {
            if (BugPanel.Height == 0)
            {
                await ShowBugPanel();
            }
            else
            {
                await HideBugMenu();
            }
            
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
            menu.WidthRequest = showReportButtons ? 126 : 42;
            await menu.ScaleYTo(1, Settings.TextMenuAnimationMs, Easing.Linear);
        }

        private async Task HideBugMenu()
        {
            await BugPanel.ScaleYTo(0, Settings.BugMenuAnimationMs, Easing.Linear);
            BugPanel.HeightRequest = 0;
        }

        private async Task ShowBugPanel()
        {
            BugPanel.ScaleY = 0;
            BugPanel.HeightRequest = 42;
            await BugPanel.ScaleYTo(1, Settings.BugMenuAnimationMs, Easing.Linear);
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

        private double GetResultStackChildHeight(View view)
        {
            var measure = view.Measure(ScrollWrapper.Width, ScrollWrapper.Height);

            return Math.Max(measure.Minimum.Height, measure.Request.Height);
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
                var ofd = OptimizedFileDataSource.Get(_state.ReadingFile);

                if (ResultStack.Children.Count > Settings.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromTop(ofd)));
                }
                else
                {
                    double scrollToAfter = 0;
                    while (scrollToAfter < Settings.ScreensPulledOnTopScroll * ResultScroll.Height)
                    {
                        var readingIndexMin = (ushort)ResultStack.Children[0].TabIndex;
                        var p = ofd.GetPrevParagraph(readingIndexMin);
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
                var ofd = OptimizedFileDataSource.Get(_state.ReadingFile);
                if (ResultStack.Children.Count > Settings.MaxParagraphCount) // run stack cleanup
                {
                    ResultScroll.Scrolled -= ResultScrollOnScrolled;
                    Task.Run(() => Device.InvokeOnMainThreadAsync(async () => await RebuildScrollFromBottom(ofd)));
                }
                else
                {
                    double addedHeight = 0;
                    while (addedHeight < Settings.ScreensPulledOnBottomScroll * ResultScroll.Height)
                    {
                        var readingIndexMax = (ushort)ResultStack.Children.Last().TabIndex;
                        var p = ofd.GetNextParagraph(readingIndexMax);
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

        private async Task RebuildScroll(OptimizedFileData ofd, ushort index, double scrollToAfter)
        {
            await RebuildScroll(false);
            var p = ofd.GetParagraph(index);
            while (!IsResultScrollReady())
            {
                if (p == null) break;
                var pView = _paragraphViewBuilder.Build(p, _state);
                ResultStack.Children.Add(pView);
                p = ofd.GetNextParagraph(p.Index);
            }
            await ResultScroll.ScrollToAsync(0, scrollToAfter, false);
            ResultScroll.IsEnabled = false;
            await ResultScrollFadeIn();
            await Task.Delay(250);
            ResultScroll.IsEnabled = true;
            ResultScroll.Scrolled += ResultScrollOnScrolled;
        }

        private async Task RebuildScrollFromTop(OptimizedFileData ofd)
        {
            var firstChildIndex = (ushort)ResultStack.Children[0].TabIndex;
            var prevP = ofd.GetPrevParagraph(firstChildIndex);
            if (prevP == null) return;
            var prevView = _paragraphViewBuilder.Build(prevP, _state);
            ResultStack.Children.Insert(0, prevView);
            var scrollToAfter = ResultStack.Children[0].Height;
            ResultScroll.Scrolled -= ResultScrollOnScrolled;
            ResultScroll.IsEnabled = false;
            await RebuildScroll(ofd, prevP.Index, scrollToAfter);
        }

        private async Task RebuildScrollFromBottom(OptimizedFileData ofd)
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
            await RebuildScroll(ofd, pIndex, scrollY);
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
                _state.ParagraphResults?.Clear();
                _state.ReadingFile = cfi.Path;
                _state.ReadingParagraphIndex = paragraphIndex;
                _selectionDecorator.ClearSelection();

                HideSearchResultBar();
                await RebuildScroll(true);

                var ofd = OptimizedFileDataSource.Get(cfi.Path);
                var p = ofd.GetPrevParagraph(paragraphIndex) ?? ofd.GetParagraph(paragraphIndex);
                while (!IsResultScrollReady())
                {
                    if (p == null) break;

                    var pView = _paragraphViewBuilder.Build(p, _state);

                    ResultStack.Children.Add(pView);

                    p = ofd.GetNextParagraph(p.Index);
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

            var ofd = OptimizedFileDataSource.Get(cfi.Path);
            var headings = ofd.Headings.ToList();
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
                    var headerText = heading.GetText(OptimizedDictionary.Instance.Words);
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

        private async Task DisplaySearchHeaders(List<SearchHeaderResult> headers)
        {
            _state.ReadingFile = null;

            HideSearchResultBar();

            foreach (var h in headers)
            {
                var corpusItem = _state.GetCurrentCorpusItem();
                var corpusItemFile = corpusItem.Files.First(cfi => cfi.Path == h.File);

                var headerText = $"{corpusItem.Name}, {corpusItemFile.Name}, {h.Text}";
                var headerLabel = new Label
                { Text = headerText, TextColor = Color.Black, FontSize = Settings.SummaryFontSize };
                ResultStack.Children.Add(headerLabel);
                var readLink = ConstructHyperlink("читать",
                    new Command(async () => await Read(corpusItemFile, h.Index)), Settings.SummaryFontSize);
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

            _state.ReadingFile = null;
            _state.ParagraphResults.Clear();
            var currentCorpusItem = _state.GetCurrentCorpusItem();

            HideSearchResultBar();

            await RebuildScroll(false);

            await ReplaceCorpusWithLoading();

            var isHeaderSearch = SearchEntry.Text.StartsWith("*");
            var searchQuery = SearchEntry.Text.TrimStart('*');
            var searchOptions = new SearchOptions(searchQuery, "", OptimizedDictionary.Instance.Reversed);
            _state.SearchOptions = searchOptions;
            CorpusButton.IsEnabled = false;

            var headerResults = new List<SearchHeaderResult>();
            if (isHeaderSearch)
            {
                await _searcher.SearchHeaders(currentCorpusItem, searchOptions, headerResults);
            }
            else
            {
                await _searcher.Search(currentCorpusItem, searchOptions, _state.ParagraphResults);
            }

            await ReplaceLoadingWithCorpus();

            CorpusButton.IsEnabled = true;

            if (_state.ParagraphResults.Count > 0)
            {
                ShowSearchResultBar();
                _state.CurrentParagraphResultIndex = 0;
                await OnCurrentParagraphResultIndexChange();
            }
            else if (headerResults.Count > 0)
            {
                await DisplaySearchHeaders(headerResults);
            }
            else
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
            if (!_state.IsWatchingSearchResults()) return;
            var paragraphResult = _state.GetCurrentSearchParagraphResult();

            PrevLabel.Text = (_state.CurrentParagraphResultIndex + 1).ToString();
            NextLabel.Text = _state.ParagraphResults.Count.ToString();

            _selectionDecorator.ClearSelection();

            HideTextMenu();

            await RebuildScroll(true);

            _state.ReadingFile = paragraphResult.File;
            var ofd = OptimizedFileDataSource.Get(paragraphResult.File);

            var p = ofd.GetParagraph(paragraphResult.Index);

            ResultStack.Children.Add(_paragraphViewBuilder.Build(p, _state));
            var prevP = ofd.GetPrevParagraph(p.Index);
            ResultStack.Children.Insert(0, _paragraphViewBuilder.Build(prevP, _state));
            var maxIndex = p.Index;
            while (!IsResultScrollReady())
            {
                var nextP = ofd.GetNextParagraph(maxIndex);
                if (nextP == null) break;
                maxIndex = nextP.Index;
                ResultStack.Children.Add(_paragraphViewBuilder.Build(nextP, _state));
            }

            await ResultScroll.ScrollToAsync(0, 20, true);

            await ResultScrollFadeIn();
        }
    }
}
