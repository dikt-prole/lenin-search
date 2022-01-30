using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.Service;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.YandexVision
{
    public class YandexVisionOcrBlockService : IOcrBlockService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<(List<OcrFeaturedBlock> Blocks, bool Success, string Error)> GetBlocksAsync(string imageFile)
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
            var responseBlocks = ocrResponse.Results[0].Results[0].TextDetection.Pages[0].Blocks;
            var featuredBlocks = new List<OcrFeaturedBlock>();

            for (var blockIndex = 0; blockIndex < responseBlocks.Count; blockIndex++)
            {
                var responseBlock = responseBlocks[blockIndex];
                var topLeft = responseBlock.BoundingBox.TopLeft.Point();
                var topRight = responseBlock.BoundingBox.TopRight.Point();
                var bottomLeft = responseBlock.BoundingBox.BottomLeft.Point();
                var bottomRight = responseBlock.BoundingBox.BottomRight.Point();

                var words = responseBlock.Lines.SelectMany(l => l.Words).ToList();
                var text = string.Join(" ", words.Select(w => w.Text));
                var totalPixelWidth = responseBlock.Lines.Sum(l => l.BoundingBox.TopRight.Point().X - l.BoundingBox.TopLeft.Point().X);
                var pixelsPerSymbol = 1.0 * totalPixelWidth / text.Length;
                var rowWidth = responseBlock.BoundingBox.TopRight.Point().X - responseBlock.BoundingBox.TopLeft.Point().X;
                var rowHeight = responseBlock.BoundingBox.BottomLeft.Point().Y - responseBlock.BoundingBox.TopLeft.Point().Y;
                var pageLines = GetPageLines(imageFile);
                var imageIndex = int.Parse(new string(Path.GetFileNameWithoutExtension(imageFile).Where(char.IsNumber).ToArray()));

                var features = new OcrBlockFeatures
                {
                    PixelsPerSymbol = pixelsPerSymbol,
                    LeftIndent = topLeft.X,
                    RightIndent = image.Width - topRight.X,
                    TopIndent = topLeft.Y,
                    BottomIndent = image.Height - bottomLeft.Y,
                    SameYLevelBlockCount = responseBlocks.Count(b => b != responseBlock && responseBlock.BoundingBox.IsSameY(b.BoundingBox)),
                    Width = rowWidth,
                    Height = rowHeight,
                    WidthToHeightRatio = 1.0 * rowWidth / rowHeight,
                    WordCount = words.Count,
                    SymbolCount = text.Length,
                    TopLineDistance = responseBlock.BoundingBox.TopLeft.Point().Y - pageLines.TopY,
                    BottomLineDistance = pageLines.BottomY - responseBlock.BoundingBox.TopLeft.Point().Y,
                    ImageIndex = imageIndex,
                    FirstLineIndent = responseBlock.Lines[0].BoundingBox.TopLeft.Point().X
                };

                var featuredBlock = new OcrFeaturedBlock
                {
                    FileName = Path.GetFileNameWithoutExtension(imageFile),
                    BlockIndex = blockIndex,
                    Text = GetText(responseBlock),
                    TopLeftX = topLeft.X,
                    TopLeftY = topLeft.Y,
                    BottomRightX = bottomRight.X,
                    BottomRightY = bottomRight.Y,
                    Features = features
                };

                featuredBlocks.Add(featuredBlock);
            }

            return (featuredBlocks, true, null);
        }

        private static (int TopY, int BottomY) GetPageLines(string imageFile)
        {
            var result = CvUtil.GetPageLines(imageFile);

            if (result.BottomLineY.HasValue && result.TopLineY.HasValue)
            {
                return (result.TopLineY.Value, result.BottomLineY.Value);
            }

            using var image = Image.FromFile(imageFile);

            var topY = result.TopLineY ?? 0;
            var bottomY = result.BottomLineY ?? image.Height;

            return (topY, bottomY);
        }

        private string GetText(OcrBlock ocrBlock)
        {
            var lineTexts = ocrBlock.Lines.Select(l => string.Join(" ", l.Words.Select(w => w.Text))).ToList();

            var sb = new StringBuilder();
            foreach (var lineText in lineTexts)
            {
                if (lineText.EndsWith("-"))
                {
                    sb.Append(lineText.TrimEnd('-'));
                }
                else
                {
                    sb.Append(lineText);
                    sb.Append(" ");
                }
            }

            return sb.ToString().TrimEnd(' ');
        }
    }
}