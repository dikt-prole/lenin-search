using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CsvHelper;
using LeninSearch.Ocr.Labeling;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class OcrBlockRowsScript : IScript
    {
        public string Id => "ocr-block-rows";
        public string Arguments => "ocr-book-folder";
        public void Execute(params string[] input)
        {
            var ocrBookFolder = input[0];
            var imagesFolder = Path.Combine(ocrBookFolder, "images");
            var jsonFolder = Path.Combine(ocrBookFolder, "json");

            var blockRows = new List<OcrBlockRow>();
            var imageFiles = Directory.GetFiles(imagesFolder);
            foreach (var imageFile in imageFiles)
            {
                var jsonFile = Path.Combine(jsonFolder, Path.GetFileNameWithoutExtension(imageFile) + ".json");
                var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));

                using (var image = Image.FromFile(imageFile))
                {
                    var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];

                    for (var blockIndex = 0; blockIndex < page.Blocks.Count; blockIndex++)
                    {
                        var pageBlock = page.Blocks[blockIndex];
                        var topLeft = pageBlock.BoundingBox.TopLeft.Point();
                        var topRight = pageBlock.BoundingBox.TopRight.Point();
                        var bottomLeft = pageBlock.BoundingBox.BottomLeft.Point();
                        var bottomRight = pageBlock.BoundingBox.BottomRight.Point();

                        double pixelsPerSymbol = 0;
                        foreach (var line in pageBlock.Lines)
                        {
                            var text = string.Join(" ", line.Words.Select(w => w.Text));
                            pixelsPerSymbol += 1.0 * (line.BoundingBox.TopRight.Point().X - line.BoundingBox.TopLeft.Point().X) / text.Length;
                        }
                        pixelsPerSymbol = pixelsPerSymbol / pageBlock.Lines.Count;

                        blockRows.Add(new OcrBlockRow
                        {
                            ImageFile = imageFile,
                            OcrJsonFile = jsonFile,
                            BlockIndex = blockIndex,

                            PixelsPerSymbol = pixelsPerSymbol,
                            LeftIndent = topLeft.X,
                            RightIndent = image.Width - topRight.X,
                            TopIndent = topLeft.Y,
                            BottomIndent = image.Height - bottomLeft.Y
                        });
                    }
                }
            }

            var csvFile = Path.Combine(ocrBookFolder, "ocr-block-rows.csv");
            using (var csv = new CsvWriter(new StreamWriter(csvFile), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(blockRows);
            }
        }
    }
}