using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book.Old
{
    public class OldBookProjectData
    {
        public List<OldBookProjectPage> Pages { get; set; }
        public static OldBookProjectData Empty()
        {
            return new OldBookProjectData { Pages = new List<OldBookProjectPage>() };
        }

        public static OldBookProjectData Load(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "ocr-data.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<OldBookProjectData>(json);
            }

            var ocrData = Empty();
            var jpegFiles = Directory.GetFiles(bookFolder, "*.jpg");
            foreach (var jpegFile in jpegFiles)
            {
                using var image = Image.FromStream(new MemoryStream(File.ReadAllBytes(jpegFile)));
                var page = new OldBookProjectPage
                {
                    Filename = Path.GetFileNameWithoutExtension(jpegFile),
                    Lines = new List<OldBookProjectLine>(),
                    Width = image.Width,
                    Height = image.Height,
                    BottomDivider = new OldBookProjectDividerLine(image.Height - 1, 0, image.Width),
                    TopDivider = new OldBookProjectDividerLine(1, 0, image.Width),
                    TitleBlocks = new List<OldBookProjectTitleBlock>(),
                    ImageBlocks = new List<OldBookProjectImageBlock>()
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

        public OldBookProjectPage GetPage(string filename)
        {
            return Pages.FirstOrDefault(p => p.Filename == filename);
        }
    }
}