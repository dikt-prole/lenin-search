using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using LeninSearch.Standard.Core.Search.TokenVarying;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.ListItems;
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
            SearchEntry.ReturnCommand = new Command(OnSearchButtonPressed);

            // paragraphs
            RefreshAllTabs();
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
            var corpusListItems =
                new ObservableCollection<CorpusListItem>(corpusItems.Select(CorpusListItem.FromCorpusItem));
            CorpusCollectionView.ItemsSource = corpusListItems;
        }

        private void BeginRefreshLibraryTab()
        {
            Task.Run(() =>
            {
                ObservableCollection<LibraryListItem> libraryListItems = null;
                var summaryResult = _apiService.GetSummaryAsync(Settings.LsiVersion).Result;
                if (summaryResult.Success)
                {
                    var summary = summaryResult.Summary;
                    var updates = new List<CorpusItem>();
                    var existingCorpusIds = Settings.GetExistingCorpusIds();
                    var existingCorpusItems = existingCorpusIds.Select(id => summary.FirstOrDefault(ci => ci.Id == id))
                        .Where(ci => ci != null).ToList();
                    foreach (var existingCi in existingCorpusItems)
                    {
                        var ciSeries = summary.Where(ci => ci.Series == existingCi.Series)
                            .OrderByDescending(ci => ci.CorpusVersion).ToList();
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
                    libraryListItems =
                        new ObservableCollection<LibraryListItem>(updates.Select(LibraryListItem.FromCorpusItem));
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    LibraryCollectionView.ItemsSource = libraryListItems;
                    LibraryCollectionView.EmptyView = summaryResult.Success
                        ? "Вы уже все скачали"
                        : "Ошибка соединения с библиотекой";
                });
            });
        }

        private void RefreshBookmarksTab()
        {
            var bookmarkListItems = BookmarkRepo.GetAll()
                .OrderByDescending(b => b.When)
                .Select(BookmarkListItem.FromBookmark).ToList();
            BookmarkCollectionView.ItemsSource = bookmarkListItems;
        }

        private void RefreshAllTabs()
        {
            RefreshCorpusTab();
            RefreshBookmarksTab();
            BeginRefreshLibraryTab();
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
            RefreshAllTabs();
            _state.ReadingFile = null;
        }

        private Button ConstructHyperlinkButton(string text, double fontSize, Action action)
        {
            var textWidth = _textMeasure.Width(text, null, (float)fontSize);
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

        private async void OnLibraryDownloadClicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var libraryListItem = imageButton.CommandParameter as LibraryListItem;

            libraryListItem.IsDownloading = true;

            var sameSeriesCorpusItems = Settings.GetExistingCorpusIds()
                .Select(cid => _lsiProvider.GetCorpusItem(cid))
                .Where(ci => ci.Series == libraryListItem.Update.Series)
                .ToList();

            // 1. create corpus folder
            var corpusFolder = Path.Combine(Settings.CorpusRoot, libraryListItem.Update.Id);
            if (Directory.Exists(corpusFolder))
            {
                Directory.Delete(corpusFolder, true);
            }
            Directory.CreateDirectory(corpusFolder);

            // 2. download files
            foreach (var cfi in libraryListItem.Update.Files)
            {
                libraryListItem.Text = $"Скачиваю: {cfi.Path}";
                var cfiBytesResult = await _apiService.GetFileBytesAsync(libraryListItem.Update.Id, cfi.Path);
                if (!cfiBytesResult.Success)
                {
                    Directory.Delete(corpusFolder, true);
                    libraryListItem.Text = $"Ошибка скачивания. {cfiBytesResult.Error}";
                    return;
                }

                await File.WriteAllBytesAsync(Path.Combine(corpusFolder, cfi.Path), cfiBytesResult.Bytes);
            }

            // 3. remove same series corpus
            foreach (var corpusItem in sameSeriesCorpusItems)
            {
                Directory.Delete(Path.Combine(Settings.CorpusRoot, corpusItem.Id), true);
            }

            await Device.InvokeOnMainThreadAsync(() =>
            {
                    // 4. remove item from library collection view
                var collection = LibraryCollectionView.ItemsSource as ObservableCollection<LibraryListItem>;
                collection.Remove(libraryListItem);

                    // 5. remove same series items from corpus collection view
                var corpusListItems = CorpusCollectionView.ItemsSource as ObservableCollection<CorpusListItem>;
                foreach (var corpusItem in sameSeriesCorpusItems)
                {
                    var corpusListItem = corpusListItems.FirstOrDefault(cli => cli.CorpusId == corpusItem.Id);
                    if (corpusListItem != null)
                    {
                        corpusListItems.Remove(corpusListItem);
                    }
                }

                    // 6. add new item to corpus collection view
                corpusListItems.Add(CorpusListItem.FromCorpusItem(libraryListItem.Update));

                    // 7. reactivate corpus
                if (sameSeriesCorpusItems.Any(ci => ci.Id == _state.CorpusId))
                {
                    ActivateCorpus(libraryListItem.Update.Id);
                }
            });
        }
    }
}
