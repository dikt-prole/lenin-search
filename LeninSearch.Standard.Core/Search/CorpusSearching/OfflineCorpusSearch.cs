using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class OfflineCorpusSearch : ICorpusSearch
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly ISearchQueryFactory _queryFactory;
        private readonly LsSearcher _searcher;

        private Dictionary<string, List<SearchQuery>> _queryCache = new Dictionary<string, List<SearchQuery>>();
        public OfflineCorpusSearch(ILsiProvider lsiProvider, ISearchQueryFactory queryFactory, int tokenIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _lsiProvider = lsiProvider;
            _queryFactory = queryFactory;
            _searcher = new LsSearcher(tokenIndexCountCutoff, resultCountCutoff);
        }

        public Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode mode)
        {
            var corpusItem = _lsiProvider.GetCorpusItem(corpusId);
            var dictionary = _lsiProvider.GetDictionary(corpusId);
            var queryKey = $"{corpusId}-{query}-{mode}";
            if (!_queryCache.ContainsKey(queryKey))
            {
                var cacheQueries = _queryFactory
                    .Construct(query, dictionary.Words, mode)
                    .OrderBy(q => q.Priority)
                    .ToList();
                foreach (var searchQuery in cacheQueries)
                {
                    searchQuery.Mode = mode;
                }
                _queryCache.Add(queryKey, cacheQueries);
            }

            var searchQueries = _queryCache[queryKey];

            Debug.WriteLine("Search queries:");
            foreach (var searchQuery in searchQueries)
            {
                Debug.WriteLine(
                    $"{searchQuery.Text}, priority={searchQuery.Priority}, mode={searchQuery.Mode}, missing={string.Join(',', searchQuery.MissingTokens)}");
            }

            var result = new SearchResult { FileResults = new Dictionary<string, List<SearchQueryResult>>() };

            var corpusFileItems = corpusItem.LsiFiles();

            foreach (var cfi in corpusFileItems)
            {
                var searchQueryResults = new List<SearchQueryResult>();
                var lsiData = _lsiProvider.GetLsiData(corpusId, cfi.Path);
                var excludeParagraphs = new HashSet<ushort>();
                foreach (var searchQuery in searchQueries.OrderBy(q => q.Priority))
                {
                    var units = InnerSearch(lsiData, searchQuery, dictionary, cfi, excludeParagraphs);
                    if (units.Any())
                    {
                        var searchQueryResult = new SearchQueryResult
                        {
                            Priority = searchQuery.Priority,
                            MissingTokens = searchQuery.MissingTokens,
                            Query = searchQuery.Text,
                            Units = units
                        };
                        searchQueryResults.Add(searchQueryResult);
                        excludeParagraphs.UnionWith(units.Select(u => u.ParagraphIndex));
                    }
                }

                if (searchQueryResults.Any())
                {
                    result.FileResults.Add(cfi.Path, searchQueryResults);
                }
            }

            return Task.FromResult(result);
        }

        private List<SearchUnit> InnerSearch(LsiData lsiData, SearchQuery searchQuery, 
            LsDictionary dictionary, CorpusFileItem corpusFileItem, HashSet<ushort> excludeParagraphs)
        {
            var results = _searcher.Search(lsiData, searchQuery, excludeParagraphs);

            foreach (var r in results)
            {
                r.SetPreviewAndTitle(lsiData, corpusFileItem, dictionary);
            }

            return results;
        }
    }
}