using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class PartialParagraphSearchResult
    {
        public PartialParagraphSearchResult()
        {
            SearchResults = new List<ParagraphSearchResult>();
        }
        public bool Success => Error == null;
        public string Error { get; set; }
        public string LastCorpusFile { get; set; }
        public List<ParagraphSearchResult> SearchResults { get; set; }
        public bool IsSearchComplete { get; set; }
    }
}
