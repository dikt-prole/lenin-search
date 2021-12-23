using System.Collections.Generic;
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
            return $"{Name} ({Files?.Count})";
        }

        public CorpusFileItem GetFileByPath(string path)
        {
            return Files.FirstOrDefault(f => f.Path == path);
        }
    }
}