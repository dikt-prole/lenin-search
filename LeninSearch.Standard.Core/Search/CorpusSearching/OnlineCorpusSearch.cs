using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
            //host = "10.0.2.2";
            //port = 5000;

            _searchUrl = $"http://{host}:{port}/api/v1/corpus/search-compressed?corpusId=[corpusId]&query=[query]&mode=[mode]";
            _httpClient = new HttpClient {Timeout = TimeSpan.FromMilliseconds(timeoutMs)};
        }

        public async Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode searchMode)
        {
            try
            {
                var queryUrlEncoded = HttpUtility.UrlEncode(query);
                var searchUrl = _searchUrl
                    .Replace("[corpusId]", corpusId)
                    .Replace("[query]", queryUrlEncoded)
                    .Replace("[mode]", searchMode.ToString());
                var response = await _httpClient.GetAsync(searchUrl);

                var contentStream = await response.Content.ReadAsStreamAsync();

                await using var brotli = new BrotliStream(contentStream, CompressionMode.Decompress);

                await using var output = new MemoryStream();
                await brotli.CopyToAsync(output);

                var responseJson = Encoding.UTF8.GetString(output.ToArray());
                var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(responseJson);
                var searchResult = searchResponse.ToSearchResult();
                return searchResult;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
                return new SearchResult
                {
                    Error = exc.Message
                };
            }
        }
    }
}