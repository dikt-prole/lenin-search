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
        private readonly bool _parallel;
        private readonly LsSearcher _searcher;

        private readonly Dictionary<string, List<SearchQuery>> _queryCache = new Dictionary<string, List<SearchQuery>>();
        public OfflineCorpusSearch(ILsiProvider lsiProvider, ISearchQueryFactory queryFactory, int tokenIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue, bool parallel = false)
        {
            _lsiProvider = lsiProvider;
            _queryFactory = queryFactory;
            _parallel = parallel;
            _searcher = new LsSearcher(tokenIndexCountCutoff, resultCountCutoff);
        }

        public async Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode mode)
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

            var result = new SearchResult { FileResults = new Dictionary<string, List<SearchQueryResult>>() };

            var corpusFileItems = corpusItem.LsiFiles();

            if (_parallel)
            {
                var tasks = corpusFileItems.Select(cfi =>
                    Task.Run(() => InnerSearchManyQueries(corpusId, cfi, searchQueries, dictionary)));
                var manyQueryResults = await Task.WhenAll(tasks);
                foreach (var manyQueryResult in manyQueryResults)
                {
                    if (manyQueryResult.Results.Any())
                    {
                        result.FileResults.Add(manyQueryResult.File, manyQueryResult.Results);
                    }
                }
            }
            else
            {
                foreach (var corpusFileItem in corpusFileItems)
                {
                    var manyQueryResult = InnerSearchManyQueries(corpusId, corpusFileItem, searchQueries, dictionary);
                    if (manyQueryResult.Results.Any())
                    {
                        result.FileResults.Add(manyQueryResult.File, manyQueryResult.Results);
                    }
                }
            }

            return result;
        }

        private List<SearchUnit> InnerSearchOneQuery(LsiData lsiData, SearchQuery searchQuery, 
            LsDictionary dictionary, CorpusFileItem corpusFileItem, HashSet<ushort> excludeParagraphs)
        {
            var results = _searcher.Search(lsiData, searchQuery, excludeParagraphs);

            foreach (var r in results)
            {
                r.SetPreviewAndTitle(lsiData, corpusFileItem, dictionary);
            }

            return results;
        }

        private (string File, List<SearchQueryResult> Results) InnerSearchManyQueries(string corpusId, CorpusFileItem corpusFileItem, List<SearchQuery> searchQueries, LsDictionary dictionary)
        {
            var searchQueryResults = new List<SearchQueryResult>();
            var lsiData = _lsiProvider.GetLsiData(corpusId, corpusFileItem.Path);
            var excludeParagraphs = new HashSet<ushort>();
            foreach (var searchQuery in searchQueries)
            {
                var units = InnerSearchOneQuery(lsiData, searchQuery, dictionary, corpusFileItem, excludeParagraphs);
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

            return (corpusFileItem.Path, searchQueryResults);
        }
    }
}