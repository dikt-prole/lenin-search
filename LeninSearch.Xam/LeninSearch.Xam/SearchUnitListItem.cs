using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Xam
{
    public class SearchUnitListItem
    {
        public string File { get; set; }
        public string CorpusId { get; set; }
        public SearchUnit SearchUnit { get; set; }
        public string Title => $"{Index}. {SearchUnit.Title}";
        public string Preview => SearchUnit.Preview;
        public ushort Index { get; set; }
    }
}