using System.Security.Principal;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Xam.ListItems
{
    public class SearchUnitListItem
    {
        public string File { get; set; }
        public string CorpusId { get; set; }
        public SearchUnit SearchUnit { get; set; }
        public string Title => $"{Index}. {SearchUnit.Title}";
        public string Query { get; set; }
        public string Preview => SearchUnit.Preview;
        public ushort Index { get; set; }
        public ushort SpanLength { get; set; }
        public string Info => $"длина совпадения: {SpanLength} слов(а)";
        public SearchUnitListItem Self => this;
    }
}