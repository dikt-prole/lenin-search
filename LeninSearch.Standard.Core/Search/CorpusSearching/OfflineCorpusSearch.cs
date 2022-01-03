using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class OfflineCorpusSearch : ICorpusSearch
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly LsSearcher _searcher;

        private Dictionary<string, SearchQuery> _queryCache = new Dictionary<string, SearchQuery>();
        public OfflineCorpusSearch(ILsiProvider lsiProvider, int tokenIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _lsiProvider = lsiProvider;
            _searcher = new LsSearcher(tokenIndexCountCutoff, resultCountCutoff);
        }

        public Task<PartialParagraphSearchResult> SearchAsync(string corpusId, string query, string lastSearchedFilePath)
        {
            var corpusItem = _lsiProvider.GetCorpusItem(corpusId);
            var dictionary = _lsiProvider.GetDictionary(corpusId).Words;

            var searchQuery = _queryCache.ContainsKey(query) ? _queryCache[query] : SearchQuery.Construct(query, dictionary);
            if (!_queryCache.ContainsKey(query))
            {
                _queryCache.Add(query, searchQuery);
            }

            var partialResult = new PartialParagraphSearchResult
            {
                IsSearchComplete = false,
                SearchResults = new List<ParagraphSearchResult>()
            };

            var corpusFileItems = corpusItem.LsiFiles();

            var searchItems = lastSearchedFilePath == null
                ? corpusFileItems
                : corpusFileItems.SkipWhile(cfi => cfi.Path != lastSearchedFilePath).Skip(1).ToList();

            foreach (var cfi in searchItems)
            {
                var results = SearchCorpusFileItem(corpusId, cfi, searchQuery, dictionary);
                if (results.Count > 0)
                {
                    partialResult.SearchResults.AddRange(results);
                    partialResult.LastCorpusFile = cfi.Path;
                    return Task.FromResult(partialResult);
                }
            }

            partialResult.IsSearchComplete = true;

            return Task.FromResult(partialResult);
        }

        private List<ParagraphSearchResult> SearchCorpusFileItem(string corpusId, CorpusFileItem cfi, SearchQuery searchQuery, string[] dictionary)
        {
            var lsiData = _lsiProvider.GetLsiData(corpusId, cfi.Path);

            var results = searchQuery.IsHeading
                ? _searcher.SearchHeadings(lsiData, searchQuery)
                : _searcher.SearchParagraphs(lsiData, searchQuery);

            foreach (var r in results)
            {
                r.File = cfi.Path;
                if (searchQuery.IsHeading)
                {
                    var heading = lsiData.HeadingParagraphs.First(h => h.Index == r.ParagraphIndex);
                    r.Text = heading.GetText(dictionary);
                }
            }

            return results;
        }
    }
}