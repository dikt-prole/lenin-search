using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;
using Newtonsoft.Json;

namespace LeninSearch.Xam.Core
{
    public class ApiService
    {
        private HttpClient _httpClient = new HttpClient();

        public async Task<(List<CorpusItem> Summary, bool Success, string Error)> GetSummaryAsync(int lsiVersion)
        {
            try
            {
                var summaryJson = await _httpClient.GetStringAsync("http://151.248.121.154:5000/corpus/summary");

                var corpusItems = JsonConvert.DeserializeObject<List<CorpusItem>>(summaryJson);

                var resultCorpusItems = corpusItems
                    .Where(ci => ci.LsiVersion == lsiVersion)
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
                var link = $"http://151.248.121.154:5000/corpus/file?corpusId={corpusId}&file={file}";

                var response = await _httpClient.GetAsync(link);

                using (var ms = new MemoryStream())
                {
                    await response.Content.CopyToAsync(ms);
                    return (ms.ToArray(), true, null);
                }
            }
            catch (Exception exc)
            {
                return (null, false, exc.Message);
            }
        }
    }
}