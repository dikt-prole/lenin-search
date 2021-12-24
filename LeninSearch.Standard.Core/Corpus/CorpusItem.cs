using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus
{
    public class CorpusItem
    {
        public string Id { get; set; }
        public string Series { get; set; }
        public int CorpusVersion { get; set; }
        public int LsiVersion { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Description { get; set; }
        public List<CorpusFileItem> Files { get; set; }

        public override string ToString()
        {
            var booksString = "книг";
            var fileCountString = (Files?.Count(cfi => cfi.Path.EndsWith(".lsi")) ?? 0).ToString();
            if (fileCountString.EndsWith("1"))
            {
                booksString = "книга";
            }

            if (fileCountString.EndsWith("2") || fileCountString.EndsWith("3") || fileCountString.EndsWith("4"))
            {
                booksString = "книги";
            }

            return $"{Name} v.{CorpusVersion} ({fileCountString} {booksString})";
        }
        public CorpusFileItem GetFileByPath(string path)
        {
            return Files.FirstOrDefault(f => f.Path == path);
        }

        public List<CorpusFileItem> LsiFiles()
        {
            return Files.Where(f => f.Path.EndsWith(".lsi")).ToList();
        }
    }
}