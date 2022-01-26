using System;
using System.IO;
using System.Net;
using System.Net.Http;
using LeninSearch.Ocr;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class OcrJsonScript : IScript
    {
        public string Id => "ocr-json";
        public string Arguments => "image-folder, json-folder";
        public void Execute(params string[] input)
        {
            var imageFolder = input[0];
            var jsonFolder = input[1];
            var apiKey = Environment.GetEnvironmentVariable("YandexApiKey");
            var httpClient = new HttpClient();

            var imageFiles = Directory.GetFiles(imageFolder);
            foreach (var imageFile in imageFiles)
            {
                Console.WriteLine($"Processing '{imageFile}'");

                var imageBytes = File.ReadAllBytes(imageFile);
                var ocrRequest = YtVisionRequest.Ocr(imageBytes);
                var ocrRequestJson = JsonConvert.SerializeObject(ocrRequest, Formatting.Indented);
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://vision.api.cloud.yandex.net/vision/v1/batchAnalyze"),
                    Method = HttpMethod.Post,
                    Headers =
                    {
                        {HttpRequestHeader.ContentType.ToString(), "application/json"},
                        {HttpRequestHeader.Authorization.ToString(), $"Api-Key {apiKey}"}
                    },
                    Content = new StringContent(ocrRequestJson)
                };

                var response = httpClient.SendAsync(request).Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Response code: {response.StatusCode}");
                    return;
                }

                var responseJson = response.Content.ReadAsStringAsync().Result;
                var ocrFile = Path.Combine(jsonFolder, Path.GetFileNameWithoutExtension(imageFile) + ".json");
                File.WriteAllText(ocrFile, responseJson);
            }
        }
    }
}