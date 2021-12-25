using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.OldShit;

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

        public List<ParagraphSearchResult> FileResults(string file)
        {
            return SearchResults.Where(r => r.File == file).ToList();
        }

        public List<string> Files()
        {
            return SearchResults.Select(r => r.File).Distinct().ToList();
        }
    }
}
