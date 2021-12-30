namespace LeninSearch.Standard.Core.Corpus
{
    public class MarkupData
    {
        public byte MarkupIndex { get; set; }
        public MarkupType MarkupType { get; set; }
        public string MarkupText { get; set; }
    }

    public enum MarkupType
    {
        Strong = 0,
        Emphasis = 1,
        StrongEmphasis = 2
    }
}