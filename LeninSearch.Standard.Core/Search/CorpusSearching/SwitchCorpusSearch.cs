using System.Linq;
using System.Threading.Tasks;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class SwitchCorpusSearch : ICorpusSearch
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly OnlineCorpusSearch _onlineSearch;
        private readonly OfflineCorpusSearch _offlineSearch;
        public SwitchCorpusSearch(ILsiProvider lsiProvider, int? batchSize, string host, int port, int timeoutMs, int tokeIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _lsiProvider = lsiProvider;
            _onlineSearch = new OnlineCorpusSearch(host, port, timeoutMs, _lsiProvider);
            _offlineSearch = new OfflineCorpusSearch(lsiProvider, batchSize, tokeIndexCountCutoff, resultCountCutoff);
        }
        public async Task<PartialParagraphSearchResult> SearchAsync(string corpusName, int corpusVersion, string query, string lastSearchedFilePath)
        {
            var onlineResult = await _onlineSearch.SearchAsync(corpusName, corpusVersion, query, lastSearchedFilePath);

            if (onlineResult.Success)
            {
                if (lastSearchedFilePath != null)
                {
                    var corpusItem = _lsiProvider.GetCorpus(corpusVersion).Items.First(ci => ci.Name == corpusName);

                    var corpusFileItems = corpusItem.Files.SkipWhile(cfi => cfi.Path != lastSearchedFilePath).Skip(1).Select(cfi => cfi.Path).ToList();

                    onlineResult.SearchResults = onlineResult.SearchResults.Where(psr => corpusFileItems.Contains(psr.File)).ToList();
                }

                return onlineResult;
            }

            var offlineResult = await _offlineSearch.SearchAsync(corpusName, corpusVersion, query, lastSearchedFilePath);

            return offlineResult;
        }
    }
}