using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Corpus
{
    public class FileData
    {
        public List<Heading> Headings { get; set; }
        public Dictionary<ushort, ushort> Pages { get; set; }
        public List<Paragraph> Pars { get; set; }
    }
}