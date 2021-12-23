using System;
using System.IO;
using System.Net.Http;

namespace LeninSearch.Script.Scripts
{
    public class YandexCloudDownloadScript : IScript
    {
        public string Id => "yandex-cloud-download";
        public void Execute(params string[] input)
        {
            var corpusId = input[0];
            var file = input[1];
            var link = $"https://lenin-search.storage.yandexcloud.net/{corpusId}/{file}";
            var tempFile = Path.Combine(Path.GetTempPath(), file);

            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(link).Result;
            using (var fs = new FileStream(tempFile, FileMode.Create))
            {
                response.Content.CopyToAsync(fs).Wait();
            }

            Console.WriteLine($"File saved to '{tempFile}'");
        }
    }
}