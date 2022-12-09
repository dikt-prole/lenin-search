using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class OnlineCorpusSearch : ICorpusSearch
    {
        private readonly IApiClientV1 _apiClientV1;

        public OnlineCorpusSearch(IApiClientV1 apiClientV1)
        {
            _apiClientV1 = apiClientV1;
        }

        public Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode searchMode)
        {
            return _apiClientV1.SearchAsync(corpusId, query, searchMode);
        }
    }
}