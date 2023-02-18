using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Domain
{
    public class Book
    {
        [JsonProperty("pgs")]
        public List<Page> Pages { get; set; }

        [JsonIgnore]
        public string Folder { get; set; }

        public void Save(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "book.json");
            var json = JsonConvert.SerializeObject(this);
            File.WriteAllText(file, json);
        }

        public static Book Load(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "book.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var book = JsonConvert.DeserializeObject<Book>(json);
                book.Folder = bookFolder;

                return book;
            }
            else
            {
                var book = new Book
                {
                    Pages = new List<Page>(),
                    Folder = bookFolder
                };
                var imageFiles = Directory.GetFiles(bookFolder, "*.jpg");
                foreach (var imageFile in imageFiles)
                {
                    book.Pages.Add(Page.ConstructEmpty(imageFile));
                }

                return book;
            }
        }

        public Page GetPage(string imageFile)
        {
            return Pages?.FirstOrDefault(p => p.ImageFile == imageFile);
        }
    }
}