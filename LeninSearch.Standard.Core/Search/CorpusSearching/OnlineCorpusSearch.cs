using System;
using System.Collections.Generic;
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
            _searchUrl = $"http://{host}:{port}/corpus/ls-search";
            _httpClient = new HttpClient {Timeout = TimeSpan.FromMilliseconds(timeoutMs)};
        }

        public async Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode searchMode)
        {
            var request = new CorpusSearchRequest
            {
                CorpusId = corpusId,
                Query = query
            };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_searchUrl, content);
                var responseJson = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonConvert.DeserializeObject<CorpusSearchResponse>(responseJson);
                var searchResult = new SearchResult { Units = new Dictionary<string, Dictionary<string, List<SearchUnit>>>() };
                foreach (var file in searchResponse.Units.Keys)
                {
                    searchResult.Units.Add(file, new Dictionary<string, List<SearchUnit>>());
                    foreach (var fileQuery in searchResponse.Units[file].Keys)
                    {
                        var units = searchResponse.Units[file][fileQuery].Select(r => r.ToSearchResultUnit()).ToList();
                        searchResult.Units[file].Add(fileQuery, units);
                    }
                }

                return searchResult;
            }
            catch (Exception exc)
            {
                return new SearchResult
                {
                    Error = exc.Message
                };
            }
        }
    }
}