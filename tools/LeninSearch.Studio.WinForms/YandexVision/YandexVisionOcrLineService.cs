using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LeninSearch.Studio.Core.Models;
using LeninSearch.Studio.WinForms.CV;
using LeninSearch.Studio.WinForms.Model;
using LeninSearch.Studio.WinForms.Service;
using Newtonsoft.Json;

namespace LeninSearch.Studio.WinForms.YandexVision
{
    public class YandexVisionOcrLineService : IOcrService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            var apiKey = Environment.GetEnvironmentVariable("YandexApiKey");
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

            OcrResponse.OcrResponse ocrResponse = null;
            try
            {
                var response = await HttpClient.SendAsync(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return (null, false, $"Send request error: {response.StatusCode}");
                }

                var ocrResponseJson = response.Content.ReadAsStringAsync().Result;
                ocrResponse = JsonConvert.DeserializeObject<OcrResponse.OcrResponse>(ocrResponseJson);
            }
            catch (Exception exc)
            {
                return (null, false, $"Send request error: {exc.Message}");
            }

            using var image = new Bitmap(Image.FromFile(imageFile));
            var responseBlocks = ocrResponse.Results[0].Results[0]?.TextDetection?.Pages[0]?.Blocks;
            var page = new OcrPage
            {
                Filename = Path.GetFileNameWithoutExtension(imageFile),
                Lines = new List<OcrLine>(),
                Width = image.Width,
                Height = image.Height,
                BottomDivider = new DividerLine(image.Height - 1, 0, image.Width),
                TopDivider = new DividerLine(1, 0, image.Width),
                ImageBlocks = new List<OcrImageBlock>()
            };

            if (responseBlocks == null)
            {
                var featuredLine = new OcrLine
                {
                    LineIndex = 0,
                    Words = null,
                    TopLeftX = 20,
                    TopLeftY = 20,
                    BottomRightX = image.Width - 40,
                    BottomRightY = image.Height - 40,
                    Features = null,
                    Label = OcrLabel.Image
                };

                page.Lines.Add(featuredLine);
                return (page, true, null);
            }

            var pageLines = responseBlocks.SelectMany(b => b.Lines).ToList();
            var falseAreas = pageLines
                .Select(l => new FalseDividerArea(l.BoundingBox.TopLeft.Point().Y - 3, l.BoundingBox.BottomRight.Point().Y + 3))
                .ToList();

            page.TopDivider = CvUtil.GetTopDivider(imageFile, falseAreas);
            page.BottomDivider = CvUtil.GetBottomDivider(imageFile, falseAreas);

            var lineIndex = 0;
            foreach (var responseLine in pageLines)
            {
                var topLeft = responseLine.BoundingBox.TopLeft.Point();
                var bottomRight = responseLine.BoundingBox.BottomRight.Point();
                var featuredLine = new OcrLine
                {
                    LineIndex = lineIndex,
                    Words = responseLine.Words.Select(w => w.ToOcrWord()).ToList(),
                    TopLeftX = topLeft.X,
                    TopLeftY = topLeft.Y,
                    BottomRightX = bottomRight.X,
                    BottomRightY = bottomRight.Y
                };

                page.Lines.Add(featuredLine);

                lineIndex++;
            }

            return (page, true, null);
        }
    }
}