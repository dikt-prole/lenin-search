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
                var cacheQueries = _queryFactory.Construct(query, dictionary.Words, mode).ToList();
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
                Debug.WriteLine(searchQuery.Text);
            }

            var result = new SearchResult { Units = new Dictionary<string, Dictionary<string, List<SearchUnit>>>() };

            var corpusFileItems = corpusItem.LsiFiles();

            foreach (var cfi in corpusFileItems)
            {
                var queryUnits = new Dictionary<string, List<SearchUnit>>();
                var lsiData = _lsiProvider.GetLsiData(corpusId, cfi.Path);
                var excludeParagraphs = new HashSet<ushort>();
                foreach (var searchQuery in searchQueries)
                {
                    var units = InnerSearch(lsiData, searchQuery, dictionary, cfi, excludeParagraphs);
                    if (units.Any())
                    {
                        queryUnits.Add(searchQuery.Text, units);
                        excludeParagraphs.UnionWith(units.Select(u => u.ParagraphIndex));
                    }
                }

                if (queryUnits.Any())
                {
                    result.Units.Add(cfi.Path, queryUnits);
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