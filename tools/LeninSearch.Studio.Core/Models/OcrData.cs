using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LeninSearch.Studio.Core.Models
{
    public class OcrData
    {
        public List<OcrPage> Pages { get; set; }
        public static OcrData Empty()
        {
            return new OcrData {Pages = new List<OcrPage>()};
        }

        public static OcrData Load(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "ocr-data.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<OcrData>(json);
            }

            var ocrData = Empty();
            var jpegFiles = Directory.GetFiles(bookFolder, "*.jpg");
            foreach (var jpegFile in jpegFiles)
            {
                using var image = Image.FromStream(new MemoryStream(File.ReadAllBytes(jpegFile)));
                var page = new OcrPage
                {
                    Filename = Path.GetFileNameWithoutExtension(jpegFile),
                    Lines = new List<OcrLine>(),
                    Width = image.Width,
                    Height = image.Height,
                    BottomDivider = new DividerLine(image.Height - 1, 0, image.Width),
                    TopDivider = new DividerLine(1, 0, image.Width),
                    TitleBlocks = new List<OcrTitleBlock>(),
                    ImageBlocks = new List<OcrImageBlock>()
                };
                ocrData.Pages.Add(page);
            }

            return ocrData;
        }

        public void Save(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "ocr-data.json");
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(file, json);
        }

        public OcrPage GetPage(string filename)
        {
            return Pages.FirstOrDefault(p => p.Filename == filename);
        }
    }
}