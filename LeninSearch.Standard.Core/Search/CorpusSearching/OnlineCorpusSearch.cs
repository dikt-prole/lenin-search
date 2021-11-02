using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;
using Newtonsoft.Json;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public class OnlineCorpusSearch : ICorpusSearch
    {
        private readonly string _searchUrl;
        private readonly HttpClient _httpClient;

        public OnlineCorpusSearch(string host, int port, int timeoutMs)
        {
            _searchUrl = $"http://{host}:{port}/corpus/search";
            _httpClient = new HttpClient {Timeout = TimeSpan.FromMilliseconds(timeoutMs)};
        }

        public async Task<PartialParagraphSearchResult> SearchAsync(string corpusName, int corpusVersion, string query, string lastSearchedFilePath)
        {
            var request = new CorpusSearchRequest
            {
                CorpusName = corpusName,
                CorpusVersion = corpusVersion,
                Query = query
            };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_searchUrl, content);
                var responseJson = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<CorpusSearchResponse>(responseJson);

                return new PartialParagraphSearchResult
                {
                    IsSearchComplete = true,
                    SearchResults = searchResponse.Results.Select(r => r.ToParagraphSearchResult()).ToList()
                };
            }
            catch (Exception exc)
            {
                return new PartialParagraphSearchResult
                {
                    Error = exc.Message
                };
            }
        }
    }
}