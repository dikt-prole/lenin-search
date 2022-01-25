using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
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

                row.Width = block.BoundingBox.TopRight.Point().X - block.BoundingBox.TopLeft.Point().X;
                row.Height = block.BoundingBox.BottomLeft.Point().Y - block.BoundingBox.TopLeft.Point().Y;
                row.WidthToHeightRatio = 1.0 * row.Width / row.Height;
                row.WordCount = words.Count;
                row.SymbolCount = text.Length;
            }

            using (var csv = new CsvWriter(new StreamWriter(csvFile), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }
        }
    }
}