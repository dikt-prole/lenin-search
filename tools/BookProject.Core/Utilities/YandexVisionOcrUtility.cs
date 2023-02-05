using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BookProject.Core.Models.Ocr;
using BookProject.Core.Models.YandexVision.Request;
using BookProject.Core.Models.YandexVision.Response;
using Newtonsoft.Json;

namespace BookProject.Core.Utilities
{
    public class YandexVisionOcrUtility : IOcrUtility
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<OcrPage> GetPageAsync(byte[] imageBytes)
        {
            var apiKey = Environment.GetEnvironmentVariable("YandexApiKey");
            var ocrRequest = YandexVisionRequest.FromImageBytes(imageBytes);
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

            var response = await HttpClient.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Error requesting Yandex Vision API");
            }

            var ocrResponseJson = await response.Content.ReadAsStringAsync();
            var yandexVisionOcrResponse = JsonConvert.DeserializeObject<YandexVisionOcrResponse>(ocrResponseJson);

            return yandexVisionOcrResponse.ToOcrPage();
        }
    }
}