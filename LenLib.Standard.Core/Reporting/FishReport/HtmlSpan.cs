using LenLib.Standard.Core.Corpus.Lsi;

namespace LenLib.Standard.Core.Reporting.FishReport
{
    public class FishReportSpan
    {
        public string Text { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public string TextColor { get; set; }

        public static FishReportSpan From(LsiSpan lsiSpan, string[] dictionaryWords)
        {
            return new FishReportSpan
            {
                Text = lsiSpan.GetText(dictionaryWords),
                Bold = lsiSpan.Type == LsiSpanType.Strong || lsiSpan.Type == LsiSpanType.SearchResult,
                Italic = lsiSpan.Type == LsiSpanType.Emphasis,
                TextColor = lsiSpan.Type == LsiSpanType.SearchResult ? "#D6181F" : "#000000"
            };
        }

        public string ToHtmlSpan()
        {
            var style = $"color:{TextColor};";

            if (Bold) style += "font-weight:bold;";

            if (Italic) style += "font-style:italic;";

            return $"<span style='{style}'>{Text}</span>";
        }
    }
}