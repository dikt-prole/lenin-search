using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LeninSearch.Ocr.CV;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.Service;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.YandexVision
{
    public class YandexVisionOcrLineService : IOcrService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            var httpClient = new HttpClient();
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
                var response = await httpClient.SendAsync(request);
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
                BottomDivider = new DividerLine(image.Height - 1, 0, image.Width),
                TopDivider = new DividerLine(1, 0, image.Width)
            };

            if (responseBlocks == null)
            {
                var featuredLine = new OcrLine
                {
                    FileName = Path.GetFileNameWithoutExtension(imageFile),
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
                var imageIndex = int.Parse(new string(Path.GetFileNameWithoutExtension(imageFile).Where(char.IsNumber).ToArray()));

                var text = string.Join(" ", responseLine.Words.Select(w => w.Text));
                var lineRectangle = responseLine.BoundingBox.Rectangle();
                var pixelsPerSymbol = 1.0 * lineRectangle.Width / text.Length;
                var lastChar = responseLine.Words.Last().Text.Last();

                var features = new OcrLineFeatures
                {
                    // geometric features
                    LeftIndent = topLeft.X,
                    RightIndent = image.Width - bottomRight.X,
                    TopIndent = topLeft.Y,
                    BottomIndent = image.Height - bottomRight.Y,
                    BelowTopDivider = page.TopDivider.Y < topLeft.Y ? 1 : 0,
                    AboveBottomDivider = page.BottomDivider.Y > topLeft.Y ? 1 : 0,
                    Width = lineRectangle.Width,
                    Height = lineRectangle.Height,
                    SameYCount = pageLines.Count(pl => pl != responseLine && responseLine.BoundingBox.IsSameY(pl.BoundingBox)),
                    WidthToHeightRatio = 1.0 * lineRectangle.Width / lineRectangle.Height,

                    // text features
                    PixelsPerSymbol = pixelsPerSymbol,
                    WordCount = responseLine.Words.Count,
                    SymbolCount = text.Length,
                    StartsWithCapital = char.IsUpper(responseLine.Words[0].Text[0]) ? 1 : 0,
                    EndsWithSymbol = (lastChar == '.' || lastChar == '!' || lastChar == '?') ? 1 : 0,

                    // other features
                    ImageIndex = imageIndex
                };

                var featuredLine = new OcrLine
                {
                    FileName = Path.GetFileNameWithoutExtension(imageFile),
                    LineIndex = lineIndex,
                    Words = responseLine.Words.Select(w => w.ToOcrWord()).ToList(),
                    TopLeftX = topLeft.X,
                    TopLeftY = topLeft.Y,
                    BottomRightX = bottomRight.X,
                    BottomRightY = bottomRight.Y,
                    Features = features,
                    Label = features.BelowTopDivider == 0
                        ? OcrLabel.Garbage
                        : features.AboveBottomDivider == 0
                            ? OcrLabel.Comment
                            : OcrLabel.PMiddle
                };

                page.Lines.Add(featuredLine);

                lineIndex++;
            }

            return (page, true, null);
        }
    }
}