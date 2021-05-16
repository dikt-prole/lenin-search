using System.Collections.Generic;
using LeninSearch.Standard.Core;

namespace LeninSearch.Xam.Core
{
    public class Corpus
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public List<CorpusItem> Items { get; set; }
    }
}