using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using LeninSearch.Ocr;
using LeninSearch.Ocr.Labeling;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class OcrFixBlockRowData : IScript
    {
        public string Id => "ocr-fix-block-row-data";
        public string Arguments => "csv-file";
        public void Execute(params string[] input)
        {
            var csvFile = input[0];
            var bookFolder = Path.GetDirectoryName(csvFile);
            var jsonFolder = Path.Combine(bookFolder, "json");
            var imageFolder = Path.Combine(bookFolder, "images");

            List<OcrBlockRow> rows = null;
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null
            };
            using (var csv = new CsvReader(new StreamReader(csvFile), csvConfig))
            {
                rows = csv.GetRecords<OcrBlockRow>().ToList();
            }

            foreach (var row in rows)
            {
                var jsonFile = Path.Combine(jsonFolder, row.FileName + ".json");
                var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));
                var block = ocrResponse.Results[0].Results[0].TextDetection.Pages[0].Blocks[row.BlockIndex];
                var words = block.Lines.SelectMany(l => l.Words).ToList();
                var text = string.Join(" ", words.Select(w => w.Text));
                var imageFile = Directory.GetFiles(imageFolder).FirstOrDefault(f => f.Contains($"{row.FileName}."));
                var pageLines = GetPageLines(imageFile);

                row.Width = block.BoundingBox.TopRight.Point().X - block.BoundingBox.TopLeft.Point().X;
                row.Height = block.BoundingBox.BottomLeft.Point().Y - block.BoundingBox.TopLeft.Point().Y;
                row.WidthToHeightRatio = 1.0 * row.Width / row.Height;
                row.WordCount = words.Count;
                row.SymbolCount = text.Length;
                row.TopLineDistance = block.BoundingBox.TopLeft.Point().Y - pageLines.TopY;
                row.BottomLineDistance = pageLines.BottomY - block.BoundingBox.TopLeft.Point().Y;
            }

            using (var csv = new CsvWriter(new StreamWriter(csvFile), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }
        }

        private (int TopY, int BottomY) GetPageLines(string imageFile)
        {
            var pageLines = CvUtil.GetPageLines(imageFile);

            if (pageLines.BottomY.HasValue && pageLines.TopY.HasValue)
            {
                return (pageLines.TopY.Value, pageLines.BottomY.Value);
            }

            using var image = Image.FromFile(imageFile);

            var topY = pageLines.TopY ?? 0;
            var bottomY = pageLines.BottomY ?? image.Height;

            return (topY, bottomY);
        }
    }
}