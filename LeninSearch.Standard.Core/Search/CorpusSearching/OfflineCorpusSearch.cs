using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class OfflineCorpusSearch : ICorpusSearch
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly int? _batchSize;
        private readonly LsSearcher _searcher;

        private Dictionary<string, SearchQuery> _queryCache = new Dictionary<string, SearchQuery>();
        public OfflineCorpusSearch(ILsiProvider lsiProvider, int? batchSize, int tokenIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _lsiProvider = lsiProvider;
            _batchSize = batchSize;
            _searcher = new LsSearcher(tokenIndexCountCutoff, resultCountCutoff);
        }

        public Task<PartialParagraphSearchResult> SearchAsync(string corpusName, int corpusVersion, string query, string lastSearchedFilePath)
        {
            var corpus = _lsiProvider.GetCorpus(corpusVersion);
            var dictionary = _lsiProvider.GetDictionary(corpusVersion).Words;

            var searchQuery = _queryCache.ContainsKey(query) ? _queryCache[query] : SearchQuery.Construct(query, dictionary);
            if (!_queryCache.ContainsKey(query))
            {
                _queryCache.Add(query, searchQuery);
            }

            var corpusItem = corpus.Items.First(ci => ci.Name == corpusName);

            var partialResult = new PartialParagraphSearchResult
            {
                IsSearchComplete = false,
                SearchResults = new List<ParagraphSearchResult>()
            };

            var corpusFileItems = lastSearchedFilePath == null
                ? corpusItem.Files
                : corpusItem.Files.SkipWhile(cfi => cfi.Path != lastSearchedFilePath).Skip(1).ToList();

            if (!_batchSize.HasValue)
            {
                foreach (var fileItem in corpusFileItems)
                {
                    var results = SearchCorpusFileItem(fileItem, searchQuery, corpusVersion, dictionary);
                    if (results.Count > 0)
                    {
                        partialResult.SearchResults.AddRange(results);
                        partialResult.LastCorpusFile = fileItem.Path;
                        return Task.FromResult(partialResult);
                    }
                }
            }
            else
            {
                for (var i = 0; i < corpusFileItems.Count; i += _batchSize.Value)
                {
                    var cfiBatch = corpusFileItems.Skip(i).Take(_batchSize.Value).ToList();
                    var tasks = cfiBatch.Select(cfi => Task.Run(() => SearchCorpusFileItem(cfi, searchQuery, corpusVersion, dictionary)));

                    var results = Task.WhenAll(tasks).Result.SelectMany(r => r).ToList();

                    if (results.Count > 0)
                    {
                        partialResult.SearchResults.AddRange(results);
                        partialResult.LastCorpusFile = cfiBatch.Last().Path;
                        return Task.FromResult(partialResult);
                    }
                }

                partialResult.LastCorpusFile = corpusFileItems.Last().Path;
            }

            partialResult.IsSearchComplete = true;

            return Task.FromResult(partialResult);
        }

        private List<ParagraphSearchResult> SearchCorpusFileItem(CorpusFileItem cfi, SearchQuery searchQuery, int corpusVersion, string[] dictionary)
        {
            var lsiData = _lsiProvider.GetLsiData(corpusVersion, cfi.Path);

            var results = searchQuery.IsHeading
                ? _searcher.SearchHeadings(lsiData, searchQuery)
                : _searcher.SearchParagraphs(lsiData, searchQuery);

            foreach (var r in results)
            {
                r.File = cfi.Path;
                if (searchQuery.IsHeading)
                {
                    var heading = lsiData.LsData.Headings.First(h => h.Index == r.ParagraphIndex);
                    r.Text = heading.GetText(dictionary);
                }
            }

            return results;
        }
    }
}