using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class BookProjectData
    {
        public List<BookProjectPage> Pages { get; set; }
        public static BookProjectData Empty()
        {
            return new BookProjectData { Pages = new List<BookProjectPage>() };
        }

        public static BookProjectData Load(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "ocr-data.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<BookProjectData>(json);
            }

            var ocrData = Empty();
            var jpegFiles = Directory.GetFiles(bookFolder, "*.jpg");
            foreach (var jpegFile in jpegFiles)
            {
                using var image = Image.FromStream(new MemoryStream(File.ReadAllBytes(jpegFile)));
                var page = new BookProjectPage
                {
                    Filename = Path.GetFileNameWithoutExtension(jpegFile),
                    Lines = new List<BookProjectLine>(),
                    Width = image.Width,
                    Height = image.Height,
                    BottomDivider = new BookProjectDividerLine(image.Height - 1, 0, image.Width),
                    TopDivider = new BookProjectDividerLine(1, 0, image.Width),
                    TitleBlocks = new List<BookProjectTitleBlock>(),
                    ImageBlocks = new List<BookProjectImageBlock>()
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

        public BookProjectPage GetPage(string filename)
        {
            return Pages.FirstOrDefault(p => p.Filename == filename);
        }
    }
}