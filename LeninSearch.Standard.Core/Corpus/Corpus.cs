using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Corpus
{
    public class Corpus
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public List<CorpusItem> Items { get; set; }
    }
}