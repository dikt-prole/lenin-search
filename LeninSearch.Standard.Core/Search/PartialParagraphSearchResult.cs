using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class PartialParagraphSearchResult
    {
        public string LastCorpusFile => SearchResults?.Select(r => r.File).LastOrDefault();
        public List<ParagraphSearchResult> SearchResults { get; set; }
        public bool IsSearchComplete { get; set; }
    }
}
