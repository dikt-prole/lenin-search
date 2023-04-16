using System;
using System.Threading.Tasks;
using LenLib.Standard.Core.Api;
using LenLib.Standard.Core.Search.TokenVarying;

namespace LenLib.Standard.Core.Search.CorpusSearching
{
    public class SwitchCorpusSearch : ICorpusSearch
    {
        private readonly Func<bool> _isInternetAvailableFunc;
        private readonly OnlineCorpusSearch _onlineSearch;
        private readonly OfflineCorpusSearch _offlineSearch;
        public SwitchCorpusSearch(ILsiProvider lsiProvider, IApiClientV1 apiClientV1, Func<bool> isInternetAvailableFunc = null, int maxResultsPerBook = int.MaxValue)
        {
            _isInternetAvailableFunc = isInternetAvailableFunc;
            _onlineSearch = new OnlineCorpusSearch(apiClientV1);
            _offlineSearch = new OfflineCorpusSearch(lsiProvider, new SearchQueryFactory(new RuPorterStemmer()), maxResultsPerBook);
        }
        public async Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode mode)
        {
            var isInternetAvailable = _isInternetAvailableFunc?.Invoke() == true;

            if (isInternetAvailable)
            {
                var onlineResult = await _onlineSearch.SearchAsync(corpusId, query, mode);
                if (onlineResult.Success)
                {
                    return onlineResult;
                }
            }

            var offlineResult = await _offlineSearch.SearchAsync(corpusId, query, mode);

            return offlineResult;
        }
    }
}