using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Optimized
{
    public class LsData
    {
        public Dictionary<ushort, LsParagraph> Paragraphs { get; set; }
        public List<LsHeading> Headings { get; set; }
        public Dictionary<ushort, ushort> Pages { get; set; }
    }
}