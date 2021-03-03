using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Core
{
    public class FileData
    {
        public Dictionary<ushort, string> Headers { get; set; }
        public List<Heading> Headings { get; set; }
        public Dictionary<ushort, ushort> Pages { get; set; }
        public List<Paragraph> Pars { get; set; }
    }
}