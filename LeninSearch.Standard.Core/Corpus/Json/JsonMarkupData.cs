namespace LeninSearch.Standard.Core.Corpus.Json
{
    public class JsonMarkupData
    {
        public string MarkupId => $"m{MarkupIndex}";
        public byte MarkupIndex { get; set; }
        public MarkupType MarkupType { get; set; }
        public string MarkupText { get; set; }
    }

    public enum MarkupType : byte
    {
        Strong = 0,
        Emphasis = 1,
        StrongEmphasis = 2
    }
}