namespace LeninSearch.Standard.Core.Corpus.Json
{
    public class JsonMarkupData
    {
        public MarkupType MarkupType { get; set; }
        public ushort MarkupSymbolStart { get; set; }
        public ushort MarkupSymbolLength { get; set; }
        public static JsonMarkupData Strong(ushort markupStart, ushort markupLength)
        {
            return new JsonMarkupData
            {
                MarkupType = MarkupType.Strong,
                MarkupSymbolStart = markupStart,
                MarkupSymbolLength = markupLength
            };
        }
        public static JsonMarkupData Emphasis(ushort markupStart, ushort markupLength)
        {
            return new JsonMarkupData
            {
                MarkupType = MarkupType.Emphasis,
                MarkupSymbolStart = markupStart,
                MarkupSymbolLength = markupLength
            };
        }
    }

    public enum MarkupType : byte
    {
        Strong = 0,
        Emphasis = 1
    }
}