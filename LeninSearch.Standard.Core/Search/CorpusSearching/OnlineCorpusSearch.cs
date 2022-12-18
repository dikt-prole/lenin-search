using System.Diagnostics;
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

        public async Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode searchMode)
        {
            var sw = new Stopwatch();
            sw.Start();

            var result = await _apiClientV1.SearchAsync(corpusId, query, searchMode);

            sw.Start();
            Debug.WriteLine($"online search elapsed: {sw.Elapsed}");

            return result;
        }
    }
}