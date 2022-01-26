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
    public class OcrFixCsvScript : IScript
    {
        public string Id => "ocr-fix-csv";
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

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var imageFile = Directory.GetFiles(imageFolder).FirstOrDefault(f => f.Contains($"{row.FileName}."));
                var jsonFile = Path.Combine(jsonFolder, row.FileName + ".json");
                var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));
                var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];
                var fixedRow = OcrBlockRow.Construct(page, row.BlockIndex, imageFile);
                rows[i] = fixedRow;
            }

            using (var csv = new CsvWriter(new StreamWriter(csvFile), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }
        }
    }
}