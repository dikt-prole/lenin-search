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
            var featuredBlocks = new List<OcrFeaturedBlock>();

            if (responseBlocks == null)
            {
                var featuredBlock = new OcrFeaturedBlock
                {
                    FileName = Path.GetFileNameWithoutExtension(imageFile),
                    BlockIndex = 0,
                    Lines = null,
                    TopLeftX = 20,
                    TopLeftY = 20,
                    BottomRightX = image.Width - 40,
                    BottomRightY = image.Height - 40,
                    Features = null
                };
                featuredBlocks.Add(featuredBlock);
                return (featuredBlocks, true, null);
            }

            for (var blockIndex = 0; blockIndex < responseBlocks.Count; blockIndex++)
            {
                var responseBlock = responseBlocks[blockIndex];
                var topDividerLine = CvUtil.GetTopDividerLine(imageFile);
                var bottomDividerLine = CvUtil.GetBottomDividerLine(imageFile);
                var blockTopLeft = responseBlock.BoundingBox.TopLeft.Point();
                var blockBottomRight = responseBlock.BoundingBox.BottomRight.Point();
                var blockBottomLineDistance = bottomDividerLine.Y - blockTopLeft.Y;
                var blockTopLineDistance = blockTopLeft.Y - topDividerLine.Y;
                var imageIndex = int.Parse(new string(Path.GetFileNameWithoutExtension(imageFile).Where(char.IsNumber).ToArray()));

                if (blockBottomLineDistance < 0) // in this case this case the block is comment for sure*
                {
                    foreach (var responseLine in responseBlock.Lines)
                    {
                        var lineTopLeft = responseLine.BoundingBox.TopLeft.Point();
                        var lineBottomRight = responseLine.BoundingBox.BottomRight.Point();
                        var words = responseLine.Words;
                        var text = string.Join(" ", words.Select(w => w.Text));
                        var totalPixelWidth = responseBlock.Lines.Sum(l => l.BoundingBox.TopRight.Point().X - l.BoundingBox.TopLeft.Point().X);
                        var pixelsPerSymbol = 1.0 * totalPixelWidth / text.Length;
                        var blockWidth = lineBottomRight.X - lineTopLeft.X;
                        var blockHeight = lineBottomRight.Y - lineTopLeft.Y;

                        var features = new OcrBlockFeatures
                        {
                            PixelsPerSymbol = pixelsPerSymbol,
                            LeftIndent = lineTopLeft.X,
                            RightIndent = image.Width - lineBottomRight.X,
                            TopIndent = lineTopLeft.Y,
                            BottomIndent = image.Height - lineBottomRight.Y,
                            SameYLevelBlockCount = 0,
                            Width = blockWidth,
                            Height = blockHeight,
                            WidthToHeightRatio = 1.0 * blockWidth / blockHeight,
                            WordCount = words.Count,
                            SymbolCount = text.Length,
                            TopLineDistance = lineTopLeft.Y - topDividerLine.Y,
                            BottomLineDistance = bottomDividerLine.Y - lineTopLeft.Y,
                            ImageIndex = imageIndex,
                            FirstLineIndent = lineTopLeft.X
                        };

                        var featuredBlock = new OcrFeaturedBlock
                        {
                            FileName = Path.GetFileNameWithoutExtension(imageFile),
                            BlockIndex = blockIndex,
                            Lines = new List<OcrLine> { responseLine.ToOcrLine() },
                            TopLeftX = lineTopLeft.X,
                            TopLeftY = lineTopLeft.Y,
                            BottomRightX = lineBottomRight.X,
                            BottomRightY = lineBottomRight.Y,
                            Features = features
                        };

                        featuredBlocks.Add(featuredBlock);
                    }
                }
                else
                {
                    var words = responseBlock.Lines.SelectMany(l => l.Words).ToList();
                    var text = string.Join(" ", words.Select(w => w.Text));
                    var totalPixelWidth = responseBlock.Lines.Sum(l => l.BoundingBox.TopRight.Point().X - l.BoundingBox.TopLeft.Point().X);
                    var pixelsPerSymbol = 1.0 * totalPixelWidth / text.Length;
                    var rowWidth = responseBlock.BoundingBox.TopRight.Point().X - responseBlock.BoundingBox.TopLeft.Point().X;
                    var rowHeight = responseBlock.BoundingBox.BottomLeft.Point().Y - responseBlock.BoundingBox.TopLeft.Point().Y;

                    var features = new OcrBlockFeatures
                    {
                        PixelsPerSymbol = pixelsPerSymbol,
                        LeftIndent = blockTopLeft.X,
                        RightIndent = image.Width - blockBottomRight.X,
                        TopIndent = blockTopLeft.Y,
                        BottomIndent = image.Height - blockBottomRight.Y,
                        SameYLevelBlockCount = responseBlocks.Count(b => b != responseBlock && responseBlock.BoundingBox.IsSameY(b.BoundingBox)),
                        Width = rowWidth,
                        Height = rowHeight,
                        WidthToHeightRatio = 1.0 * rowWidth / rowHeight,
                        WordCount = words.Count,
                        SymbolCount = text.Length,
                        TopLineDistance = blockTopLineDistance,
                        BottomLineDistance = blockTopLineDistance,
                        ImageIndex = imageIndex,
                        FirstLineIndent = responseBlock.Lines[0].BoundingBox.TopLeft.Point().X
                    };

                    var featuredBlock = new OcrFeaturedBlock
                    {
                        FileName = Path.GetFileNameWithoutExtension(imageFile),
                        BlockIndex = blockIndex,
                        Lines = responseBlock.Lines.Select(l => l.ToOcrLine()).ToList(),
                        TopLeftX = blockTopLeft.X,
                        TopLeftY = blockTopLeft.Y,
                        BottomRightX = blockBottomRight.X,
                        BottomRightY = blockBottomRight.Y,
                        Features = features
                    };

                    featuredBlocks.Add(featuredBlock);
                }
            }

            return (featuredBlocks, true, null);
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