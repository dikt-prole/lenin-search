using System;
using System.Net.Http;
using System.Text;
using LeninSearch.Standard.Core.Api;
using Newtonsoft.Json;

namespace LeninSearch.Api.Health
{
    class Program
    {
        private static readonly string SearchUrl = $"http://151.248.121.154:5000/corpus/lssearch";
        private static readonly HttpClient HttpClient = new HttpClient{Timeout = TimeSpan.FromMinutes(10)};
        static void Main(string[] args)
        {
            var request = new SearchRequest
            {
                CorpusId = "lenin-v2",
                Query = "дикт* прол* + латин* науч*"
            };

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var response = HttpClient.PostAsync(SearchUrl, content).Result;
                var responseJson = response.Content.ReadAsStringAsync().Result;
                var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(responseJson);
                if (searchResponse?.Units.Count != 1)
                {
                    throw new Exception("Results are invalid");
                }
            }
            catch (Exception exc)
            {
                Console.Beep(800, 300);
                Console.WriteLine(exc.ToString());
                Console.ReadLine();
            }
        }
    }
}
