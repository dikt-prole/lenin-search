using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;
using LeninSearch.Standard.Core.Search.TokenVarying;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class SwitchCorpusSearch : ICorpusSearch
    {
        private readonly OnlineCorpusSearch _onlineSearch;
        private readonly OfflineCorpusSearch _offlineSearch;
        public SwitchCorpusSearch(ILsiProvider lsiProvider, IApiClientV1 apiClientV1, int tokeIndexCountCutoff = int.MaxValue, int resultCountCutoff = int.MaxValue)
        {
            _onlineSearch = new OnlineCorpusSearch(apiClientV1);
            _offlineSearch = new OfflineCorpusSearch(lsiProvider, new SearchQueryFactory(new RuPorterStemmer()), tokeIndexCountCutoff, resultCountCutoff);
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