using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using LeninSearch.Ocr.Labeling;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Script.Scripts
{
    public class OcrCreateCsvScript : IScript
    {
        public string Id => "ocr-create-csv";
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
                var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];

                if (page.Blocks == null) continue;

                for (var blockIndex = 0; blockIndex < page.Blocks.Count; blockIndex++)
                {
                    blockRows.Add(OcrBlockRow.Construct(page, blockIndex, imageFile));
                }
            }

            var csvFile = Path.Combine(ocrBookFolder, "ocr-block-rows.csv");
            if (File.Exists(csvFile)) File.Delete(csvFile);
            using (var csv = new CsvWriter(new StreamWriter(csvFile), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(blockRows);
            }
        }
    }
}