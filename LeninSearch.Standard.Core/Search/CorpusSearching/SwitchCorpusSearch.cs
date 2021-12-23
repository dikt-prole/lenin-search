using System.Linq;
using System.Threading.Tasks;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class SwitchCorpusSearch : ICorpusSearch
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly OnlineCorpusSearch _onlineSearch;
        private readonly OfflineCorpusSearch _offlineSearch;
        public SwitchCorpusSearch(ILsiProvider lsiProvider, string host, int port, int timeoutMs, int tokeIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _lsiProvider = lsiProvider;
            _onlineSearch = new OnlineCorpusSearch(host, port, timeoutMs, _lsiProvider);
            _offlineSearch = new OfflineCorpusSearch(lsiProvider, tokeIndexCountCutoff, resultCountCutoff);
        }
        public async Task<PartialParagraphSearchResult> SearchAsync(string corpusId, string query, string lastSearchedFilePath)
        {
            var onlineResult = await _onlineSearch.SearchAsync(corpusId, query, lastSearchedFilePath);

            if (onlineResult.Success)
            {
                if (lastSearchedFilePath != null)
                {
                    var corpusItem = _lsiProvider.GetCorpusItem(corpusId);

                    var corpusFileItems = corpusItem.Files.SkipWhile(cfi => cfi.Path != lastSearchedFilePath).Skip(1).Select(cfi => cfi.Path).ToList();

                    onlineResult.SearchResults = onlineResult.SearchResults.Where(psr => corpusFileItems.Contains(psr.File)).ToList();
                }

                return onlineResult;
            }

            var offlineResult = await _offlineSearch.SearchAsync(corpusId, query, lastSearchedFilePath);

            return offlineResult;
        }
    }
}