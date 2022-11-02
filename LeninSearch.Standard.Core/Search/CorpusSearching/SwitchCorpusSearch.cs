using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Search.TokenVarying;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class SwitchCorpusSearch : ICorpusSearch
    {
        private readonly OnlineCorpusSearch _onlineSearch;
        private readonly OfflineCorpusSearch _offlineSearch;
        public SwitchCorpusSearch(ILsiProvider lsiProvider, string host, int port, int timeoutMs, int tokeIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _onlineSearch = new OnlineCorpusSearch(host, port, timeoutMs);
            _offlineSearch = new OfflineCorpusSearch(lsiProvider, new SearchQueryFactory(), tokeIndexCountCutoff, resultCountCutoff);
        }
        public async Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode mode)
        {
            var onlineResult = await _onlineSearch.SearchAsync(corpusId, query, mode);

            if (onlineResult.Success)
            {
                return onlineResult;
            }

            var offlineResult = await _offlineSearch.SearchAsync(corpusId, query, mode);

            return offlineResult;
        }
    }
}