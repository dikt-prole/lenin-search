namespace LeninSearch.Core
{
    public class SummaryLine
    {
        public SummaryLine() { }

        public SummaryLine(string text, float leftIndent)
        {
            Text = text;
            LeftIndent = leftIndent;
        }

        public string Text { get; set; }
        public float LeftIndent { get; set; }
    }
}