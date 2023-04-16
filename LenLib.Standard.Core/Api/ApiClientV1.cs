using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Search;
using Newtonsoft.Json;

namespace LenLib.Standard.Core.Api
{
    public class ApiClientV1 : IApiClientV1
    {
        private readonly HttpClient _httpClient;
        private readonly string _fileLinkTemplate;
        private readonly string _summaryLink;
        private readonly string _searchUrl;

        public ApiClientV1(string host, int port, int timeoutMs)
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMilliseconds(timeoutMs) };
            _fileLinkTemplate = $"http://{host}:{port}/api/v1/corpus/file-compressed?corpusId=[corpusId]&file=[file]";
            _summaryLink = $"http://{host}:{port}/api/v1/corpus/summary";
            _searchUrl = $"http://{host}:{port}/api/v1/corpus/search-compressed?corpusId=[corpusId]&query=[query]&mode=[mode]";
        }

        public async Task<(List<CorpusItem> Summary, bool Success, string Error)> GetSummaryAsync(int lsiVersion)
        {
            try
            {
                var summaryJson = await _httpClient.GetStringAsync(_summaryLink);

                var corpusItems = JsonConvert.DeserializeObject<List<CorpusItem>>(summaryJson);

                var resultCorpusItems = corpusItems
                    .Where(ci => ci.LsiVersion <= lsiVersion)
                    .ToList();

                return (resultCorpusItems, true, null);
            }
            catch (Exception exc)
            {
                return (null, false, exc.Message);
            }
        }

        public (List<CorpusItem> Summary, bool Success, string Error) GetSummary(int lsiVersion)
        {
            try
            {
                var summaryJson = _httpClient.GetStringAsync(_summaryLink).Result;

                var corpusItems = JsonConvert.DeserializeObject<List<CorpusItem>>(summaryJson);

                var resultCorpusItems = corpusItems
                    .Where(ci => ci.LsiVersion <= lsiVersion)
                    .ToList();

                return (resultCorpusItems, true, null);
            }
            catch (Exception exc)
            {
                return (null, false, exc.Message);
            }
        }

        public async Task<(byte[] Bytes, bool Success, string Error)> GetFileBytesAsync(string corpusId, string file)
        {
            try
            {
                var fileLink = _fileLinkTemplate.Replace("[corpusId]", corpusId).Replace("[file]", file);
                var response = await _httpClient.GetAsync(fileLink);

                var responseBytes = await Decompress(response);

                return (responseBytes, true, null);
            }
            catch (Exception exc)
            {
                return (null, false, exc.Message);
            }
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

                var responseBytes = await Decompress(response);

                var responseJson = Encoding.UTF8.GetString(responseBytes);
                var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(responseJson);
                var searchResult = searchResponse.ToSearchResult();
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

        private async Task<byte[]> Decompress(HttpResponseMessage responseMessage)
        {
            var contentStream = await responseMessage.Content.ReadAsStreamAsync();

            await using var brotli = new BrotliStream(contentStream, CompressionMode.Decompress);

            await using var output = new MemoryStream();
            await brotli.CopyToAsync(output);

            return output.ToArray();
        }
    }
}