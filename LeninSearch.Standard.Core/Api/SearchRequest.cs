using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Standard.Core.Api
{
    public class SearchRequest
    {
        public string CorpusId { get; set; }
        public string Query { get; set; }
        public SearchMode Mode { get; set; }
    }
}