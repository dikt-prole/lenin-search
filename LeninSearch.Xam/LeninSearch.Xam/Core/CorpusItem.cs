using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeninSearch.Xam.Core
{
    public class CorpusItem
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Description { get; set; }
        public List<CorpusFileItem> Files { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Files?.Count})";
        }

        public CorpusFileItem GetFileByPath(string path)
        {
            return Files.FirstOrDefault(f => f.Path == path);
        }
    }
}