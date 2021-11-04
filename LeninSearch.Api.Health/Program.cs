using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using LeninSearch.Standard.Core.Api;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Api.Health
{
    class Program
    {
        private static readonly string SearchUrl = $"http://151.248.121.154:5000/corpus/search";
        private static readonly HttpClient HttpClient = new HttpClient{Timeout = TimeSpan.FromSeconds(3)};
        static void Main(string[] args)
        {
            var request = new CorpusSearchRequest
            {
                CorpusName = "Ленин ПСС",
                CorpusVersion = 1,
                Query = "дикт* прол* + латин* науч*"
            };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = HttpClient.PostAsync(SearchUrl, content).Result;
                var responseJson = response.Content.ReadAsStringAsync().Result;
                var searchResponse = JsonConvert.DeserializeObject<CorpusSearchResponse>(responseJson);
                if (searchResponse?.Results.Count != 1)
                {
                    throw new Exception("Results are invalid");
                }
            }
            catch
            {
                Console.Beep(800, 300);
            }
        }
    }
}
