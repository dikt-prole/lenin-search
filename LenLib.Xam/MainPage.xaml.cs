﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LenLib.Standard.Core.Api;
using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Corpus.Lsi;
using LenLib.Standard.Core.Search;
using LenLib.Standard.Core.Search.CorpusSearching;
using LenLib.Xam.Core;
using LenLib.Xam.Core.Interfaces;
using LenLib.Xam.ListItems;
using LenLib.Xam.Services;
using LenLib.Xam.State;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace LenLib.Xam
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly GlobalEvents _globalEvents;
        private readonly CachedLsiProvider _lsiProvider;
        private readonly ICorpusSearch _corpusSearch;
        private readonly ApiClientV1 _apiClientV1 = new ApiClientV1(Options.Web.Host, Options.Web.Port, Options.Web.TimeoutMs);
        private readonly IMessage _message = DependencyService.Get<IMessage>();
        private AppState _state = AppState.Default();
        private readonly ILibraryDownloadManager _libraryDownloadManager;
        public const int ReadSwapCount = 20;
        private readonly ReadSwapHelper _readSwap;

        public MainPage(GlobalEvents globalEvents)
        {
            _globalEvents = globalEvents;
            InitializeComponent();

            _lsiProvider = new CachedLsiProvider();

            _corpusSearch = new SwitchCorpusSearch(
                _lsiProvider, 
                _apiClientV1, 
                () => Connectivity.NetworkAccess == NetworkAccess.Internet, 
                Options.Search.MaxResultsPerBook);

            //_corpusSearch = new OnlineCorpusSearch(Settings.Web.Host, Settings.Web.Port, Settings.Web.TimeoutMs);

            //_corpusSearch = new OfflineCorpusSearch(_lsiProvider,
            //    new SearchQueryFactory(new RuPorterStemmer()),
            //    Settings.TokenIndexCountCutoff,
            //    Settings.ResultCountCutoff);

            SearchEntry.ReturnCommand = new Command(OnSearchButtonPressed);

            _readSwap = new ReadSwapHelper(
                _lsiProvider,
                async s => await DisplayAlert("", s, "OK", FlowDirection.MatchParent),
                () => (ushort)MainTabs.Width,
                OnReadItemTapped);
            ReadCollectionView.Scrolled += OnReadCollectionViewScrolled;

            _libraryDownloadManager = new LibraryDownloadManager(_lsiProvider, _apiClientV1);
            _libraryDownloadManager.Error += OnLibraryDownloadManagerError;
            _libraryDownloadManager.Progress += OnLibraryDownloadManagerProgress;
            _libraryDownloadManager.Completed += OnLibraryDownloadManagerCompleted;
        }

        private void OnLibraryDownloadManagerCompleted(object sender, LibraryDownloadCompleteEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var libraryListItems = LibraryCollectionView.ItemsSource as ObservableCollection<LibraryListItem>;
                var libraryListItem = libraryListItems.FirstOrDefault(i => i.Update.Id == e.Download.Id);

                if (libraryListItem != null)
                {
                    libraryListItems.Remove(libraryListItem);

                    var corpusListItems = CorpusCollectionView.ItemsSource as ObservableCollection<CorpusListItem>;
                    var removedCorpusListItems = corpusListItems.Where(i => e.Removed.Any(ci => ci.Id == i.CorpusId)).ToList();
                    foreach (var removedCorpusListItem in removedCorpusListItems)
                    {
                        corpusListItems.Remove(removedCorpusListItem);
                    }

                    var addedCorpusListItem = CorpusListItem.FromCorpusItem(e.Download);
                    corpusListItems.Add(addedCorpusListItem);

                    if (removedCorpusListItems.Any(ci => ci.CorpusId == _state.ActiveCorpusId))
                    {
                        ActivateCorpus(e.Download.Id);
                    }
                }
            });
        }

        private void OnLibraryDownloadManagerProgress(object sender, LibraryDownloadProgressEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var libraryListItems = LibraryCollectionView.ItemsSource as ObservableCollection<LibraryListItem>;
                var libraryListItem = libraryListItems.FirstOrDefault(i => i.Update.Id == e.Download.Id);
                if (libraryListItem != null)
                {
                    libraryListItem.Text = $"Скачиваю: {e.File}";
                }
            });
        }

        private void OnLibraryDownloadManagerError(object sender, LibraryDownloadErrorEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var libraryListItems = LibraryCollectionView.ItemsSource as ObservableCollection<LibraryListItem>;
                var libraryListItem = libraryListItems.FirstOrDefault(i => i.Update.Id == e.Download.Id);
                if (libraryListItem != null)
                {
                    libraryListItem.Text = "Ошибка скачивания";
                    libraryListItem.IsDownloading = false;
                }
            });
        }

        private void OnReadCollectionViewScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            var readListItems = ReadCollectionView.ItemsSource as ObservableCollection<ReadListItem>;
            if (readListItems != null && e.FirstVisibleItemIndex < readListItems.Count)
            {
                var currentReadListItem = readListItems[e.CenterItemIndex];
                _state.ReadingTabState.SelectedParagraphIndex = currentReadListItem.ParagraphIndex;

                var bottomReadListItem = readListItems[e.LastVisibleItemIndex];
                if (readListItems.Last() == bottomReadListItem)
                {
                    var nextReadSwap = _readSwap.GetNextReadSwap(
                        bottomReadListItem.CorpusId,
                        bottomReadListItem.File,
                        bottomReadListItem.ParagraphIndex);
                    foreach (var readListItem in nextReadSwap)
                    {
                        readListItems.Add(readListItem);
                    }
                }

                var topReadListItem = readListItems[e.FirstVisibleItemIndex];
                if (readListItems.First() == topReadListItem)
                {
                    var prevReadSwap = _readSwap.GetPrevReadSwap(
                        topReadListItem.CorpusId,
                        topReadListItem.File,
                        topReadListItem.ParagraphIndex);
                    prevReadSwap.Reverse();
                    foreach (var readListItem in prevReadSwap)
                    {
                        readListItems.Insert(0, readListItem);
                    }
                }
            }
        }

        private void ActivateCorpus(string corpusId, bool alert = true)
        {
            CorpusTabViewItem.Icon = Options.IconFile(corpusId);
            _state.ActiveCorpusId = corpusId;

            var corpusItem = _lsiProvider.GetCorpusItem(corpusId);
            var summaryBookListItems = SummaryBookListItem.Construct(corpusItem).ToList();
            SummaryBookPicker.ItemsSource = summaryBookListItems;
            SummaryBookPicker.SelectedIndex = 0;
            _state.SummaryTabState.SelectedFile = summaryBookListItems[0].File;

            SearchCollectionView.ItemsSource = null;
            _state.SearchTabState.SearchResult = null;

            ReadCollectionView.ItemsSource = null;
            _state.ReadingTabState = null;
            
            if (alert)
            {
                _message.ShortAlert($"Активирован корпус '{corpusItem.Name}'");
            }
        }

        public void CleanCache()
        {
            _lsiProvider.CleanCache();
        }

        public void SetState(AppState state)
        {
            RefreshAllTabs();
            MainTabs.SelectedIndex = state.SelectedTabIndex;
            var corpusItem = state.GetCurrentCorpusItem();
            ActivateCorpus(corpusItem.Id, false);

            // summary tab
            var summarySelectedFile = state.SummaryTabState?.SelectedFile;
            if (summarySelectedFile != null)
            {
                var summaryBookListItems = SummaryBookPicker.ItemsSource as List<SummaryBookListItem>;
                if (summaryBookListItems != null)
                {
                    var selectedSummaryBookListItem = summaryBookListItems.FirstOrDefault(sli => sli.File == summarySelectedFile);
                    if (selectedSummaryBookListItem != null)
                    {
                        SummaryBookPicker.SelectedIndex = summaryBookListItems.IndexOf(selectedSummaryBookListItem);
                    }
                }
            }
            else
            {
                SummaryBookPicker.SelectedIndex = 0;
            }

            // read tab
            var selectedFile = state.ReadingTabState?.SelectedFile;
            var selectedParagraphIndex = state.ReadingTabState?.SelectedParagraphIndex;
            if (!string.IsNullOrEmpty(selectedFile) && selectedParagraphIndex.HasValue)
            {
                RunReadBook(state.ActiveCorpusId, selectedFile, selectedParagraphIndex.Value, false);
            }

            // search tab
            SearchEntry.Text = state.SearchTabState?.SearchQuery;
            SearchCollectionView.ItemsSource = null;
            var searchResult = state.SearchTabState?.SearchResult;
            if (searchResult != null)
            {
                var searchUnitListItems = SearchUnitListItem.FromSearchResult(searchResult, state.ActiveCorpusId);
                SearchCollectionView.ItemsSource = searchUnitListItems;
            }

            _state = state;
        }

        private void GlobalEventsOnBackButtonPressed(object sender, EventArgs e)
        { }

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
            var corpusItems = Options.GetCorpusItems().ToList();
            if (corpusItems.Any())
            {
                NoCorpusStackLayout.IsVisible = false;
                CorpusCollectionView.IsVisible = true;
                var corpusListItems =
                    new ObservableCollection<CorpusListItem>(corpusItems.Select(CorpusListItem.FromCorpusItem));
                CorpusCollectionView.ItemsSource = corpusListItems;
            }
            else
            {
                NoCorpusStackLayout.IsVisible = true;
                CorpusCollectionView.IsVisible = false;
            }
        }

        private void RunRefreshLibraryTab()
        {
            Task.Run(() =>
            {
                ObservableCollection<LibraryListItem> libraryListItems = null;
                var summaryResult = _apiClientV1.GetSummaryAsync(Options.LsiVersion).Result;
                if (summaryResult.Success)
                {
                    var summary = summaryResult.Summary;
                    var updates = new List<CorpusItem>();
                    var finishedCorpusIds = Options.GetFinishedCorpusIds();
                    var existingCorpusItems = finishedCorpusIds.Select(id => summary.FirstOrDefault(ci => ci.Id == id))
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
                    LibraryCollectionView.EmptyView = "Вы уже все скачали";
                    if (LibraryCollectionView.ItemsSource == null)
                    {
                        LibraryCollectionView.ItemsSource = libraryListItems;
                        if (!summaryResult.Success)
                        {
                            LibraryCollectionView.EmptyView = "Ошибка соединения с библиотекой";
                        }
                    }
                    else if (summaryResult.Success)
                    {
                        var currentLibraryListItems = LibraryCollectionView.ItemsSource as ObservableCollection<LibraryListItem>;
                        var missingLibraryListItems = libraryListItems
                            .Where(i => currentLibraryListItems.All(ci => ci.Update.Id != i.Update.Id))
                            .ToList();
                        foreach (var libraryListItem in missingLibraryListItems)
                        {
                            currentLibraryListItems.Add(libraryListItem);
                        }
                    }
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
            RunRefreshLibraryTab();
        }

        private void OnSearchButtonPressed()
        {
            if (string.IsNullOrWhiteSpace(SearchEntry.Text)) return;

            // 1. Initial stuff
            SearchEntry.Text = new SearchQueryCleaner().Clean(SearchEntry.Text);
            _state.SearchTabState ??= new SearchTabState();
            _state.SearchTabState.SearchQuery = SearchEntry.Text;
            ReplaceSearchModeButtonWithLoading();

            Task.Run(async () =>
            {
                // 1. Save history
                var currentCorpusItem = _state.GetCurrentCorpusItem();
                var historyItem = new HistoryItem
                {
                    CorpusName = currentCorpusItem.Name,
                    CorpusId = currentCorpusItem.Id,
                    Query = SearchEntry.Text,
                    QueryDateUtc = DateTime.UtcNow
                };
                HistoryRepo.AddHistory(historyItem);

                var searchMode = GetCurrentSearchMode();
                var searchResult = await _corpusSearch.SearchAsync(currentCorpusItem.Id, SearchEntry.Text, searchMode);
                _state.SearchTabState.SearchResult = searchResult;

                if (!searchResult.Success)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ReplaceLoadingWithSearchModeButton();
                        SearchCollectionView.EmptyView = "При поиске произошла ошибка";
                        SearchCollectionView.ItemsSource = null;
                    });
                }
                else
                {
                    var searchUnitListItems = SearchUnitListItem.FromSearchResult(searchResult, currentCorpusItem.Id);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ReplaceLoadingWithSearchModeButton();
                        SearchCollectionView.EmptyView = "Ничего не найдено, попробуйте другой запрос";
                        SearchCollectionView.ItemsSource = searchUnitListItems;
                        _message.ShortAlert($"Найдено {searchUnitListItems.Count} совпадений");
                    });
                }
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

            // highlight
            var searchUnitListItems = SearchCollectionView.ItemsSource as List<SearchUnitListItem>;
            foreach (var i in searchUnitListItems)
            {
                if (i.IsHighlighted)
                {
                    i.IsHighlighted = false;
                }
            }
            searchUnitListItem.IsHighlighted = true;

            RunReadBook(searchUnitListItem.CorpusId, searchUnitListItem.File, searchUnitListItem.SearchUnit.ParagraphIndex, true, searchUnitListItem.SearchUnit);
        }

        private void SummaryBookPickerOnSelectedIndexChanged(object sender, EventArgs e)
        {
            var summaryBookListItem = SummaryBookPicker.SelectedItem as SummaryBookListItem;
            if (summaryBookListItem == null) return;

            _state.SummaryTabState ??= new SummaryTabState();
            _state.SummaryTabState.SelectedFile = summaryBookListItem.File;

            var lsiData = _lsiProvider.GetLsiData(summaryBookListItem.CorpusId, summaryBookListItem.File);
            var dictionary = _lsiProvider.GetDictionary(summaryBookListItem.CorpusId);
            var summaryListItems = lsiData.HeadingParagraphs.OrderBy(h => h.Index).Select(hp => new SummaryListItem
            {
                CorpusId = summaryBookListItem.CorpusId,
                File = summaryBookListItem.File,
                ParagraphIndex = hp.Index,
                Title = hp.GetText(dictionary.Words),
                Padding = new Thickness(10 + 20 * hp.HeadingLevel, 0, 10, 0)
            }).ToList();
            SummaryCollectionView.ItemsSource = summaryListItems;
        }

        private void OnSummaryItemTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            Animations.OpacityToZeroAndBack(label, 500);
            var summaryListItem = (label.GestureRecognizers[0] as TapGestureRecognizer).CommandParameter as SummaryListItem;
            RunReadBook(summaryListItem.CorpusId, summaryListItem.File, summaryListItem.ParagraphIndex);
        }

        private void OnReadItemTapped(ReadListItem readListItem)
        {
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
            //MainTabs.IsSwipeEnabled = readListItem.ImageZoomCoefficient == 1;
        }

        private void OnMainTabsSelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            // hack: restore swipe after image zoom
            //MainTabs.IsSwipeEnabled = true;
            _state.SelectedTabIndex = MainTabs.SelectedIndex;
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

        private void RunReadBook(string corpusId, string file, ushort selectedParagraphIndex, bool selectTab = true, SearchUnit searchUnit = null)
        {
            Task.Run(() =>
            {
                _state.ReadingTabState ??= new ReadingTabState();
                _state.ReadingTabState.SelectedParagraphIndex = selectedParagraphIndex;
                _state.ReadingTabState.SelectedFile = file;

                var initialReadSwap = _readSwap.GetInitialReadSwap(corpusId, file, selectedParagraphIndex, searchUnit);
                var corpusFileItem = _lsiProvider.GetCorpusItem(corpusId)
                    .GetFileByPath(file);
                var scrollTo = initialReadSwap.FirstOrDefault(i => i.ParagraphIndex == selectedParagraphIndex);
                Device.BeginInvokeOnMainThread(() =>
                {
                    ReadBookTitleLabel.Text = corpusFileItem.Name;
                    ReadCollectionView.ItemsSource = new ObservableCollection<ReadListItem>(initialReadSwap);
                    if (selectTab)
                    {
                        MainTabs.SelectedIndex = MainTabs.TabItems.IndexOf(ReadTabViewItem);
                    }
                    Task.Run(() =>
                    {
                        Task.Delay(500);
                        if (scrollTo != null)
                        {
                            ReadCollectionView.ScrollTo(scrollTo, null, ScrollToPosition.Center, false);
                        }
                    });
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
                Title = "LenLib Share"
            }).Wait();
        }

        private void OnLibraryDownloadClicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var libraryListItem = imageButton.CommandParameter as LibraryListItem;
            libraryListItem.IsDownloading = true;
            _libraryDownloadManager.StartDownload(libraryListItem.Update);
        }

        private async void OnDeleteCorpusClicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            var corpusListItem = imageButton.CommandParameter as CorpusListItem;

            if (corpusListItem.CorpusId == _state.ActiveCorpusId)
            {
                await DisplayAlert("", "Невозможно удалить активный корпус", "OK");
                return;
            }

            var alertResult = await DisplayAlert("", $"Удалить '{corpusListItem.CorpusTitle}'?", "OK", "ОТМЕНА");

            if (!alertResult) return;

            var corpusFolder = Path.Combine(Options.CorpusRoot, corpusListItem.CorpusId);
            if (Directory.Exists(corpusFolder))
            {
                Directory.Delete(corpusFolder, true);
            }

            var corpusListItems = CorpusCollectionView.ItemsSource as ObservableCollection<CorpusListItem>;
            corpusListItems.Remove(corpusListItem);

            RunRefreshLibraryTab();
        }

        private void OnCancelLibraryDownload(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var activityIndicator = sender as ActivityIndicator;
                var gestureRecognizer = activityIndicator.GestureRecognizers[0] as TapGestureRecognizer;
                var libraryListItem = gestureRecognizer.CommandParameter as LibraryListItem;
                _libraryDownloadManager.CancelDownload(libraryListItem.Update.Id);
                libraryListItem.IsDownloading = false;
                libraryListItem.Text = libraryListItem.Update.ToString();
            });
        }

        private async void OnTgButtonClicked(object sender, EventArgs e)
        {
            var openResult = await Launcher.TryOpenAsync(Options.Web.TelegramLink);
            if (!openResult)
            {
                _message.ShortAlert("Telegram не установлен");
                await Task.Delay(500);
                await Launcher.TryOpenAsync("https://play.google.com/store/apps/details?id=org.telegram.messenger");
            }
        }

        private void OnFontSizeButtonClicked(object sender, EventArgs e)
        {
            var readItemList = ReadCollectionView.ItemsSource as ObservableCollection<ReadListItem>;

            if (readItemList == null)
            {
                return;
            }

            var currentFontSize = Settings.Instance.FontSize;
            var currentSizeIndex = Options.UI.Fonts.Sizes.IndexOf(currentFontSize);
            var nextFontSizeIndex = (currentSizeIndex + 1) % Options.UI.Fonts.Sizes.Length;
            Settings.Instance.FontSize = Options.UI.Fonts.Sizes[nextFontSizeIndex];
            Settings.Instance.Save();

            foreach (var item in readItemList)
            {
                item.FontSize = Settings.Instance.FontSize;
            }
        }
    }
}
