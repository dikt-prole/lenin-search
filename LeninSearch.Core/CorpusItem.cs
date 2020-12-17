using System.Collections.Generic;

namespace LeninSearch.Core
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
    }
}