using Xamarin.Forms;

namespace LeninSearch.Xam.ListItems
{
    public class SummaryListItem
    {
        public ushort ParagraphIndex { get; set; }
        public string CorpusId { get; set; }
        public string File { get; set; }
        public string Title { get; set; }
        public Thickness Padding { get; set; }
        public SummaryListItem Self => this;
    }
}