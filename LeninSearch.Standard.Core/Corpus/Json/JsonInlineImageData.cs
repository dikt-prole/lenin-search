namespace LeninSearch.Standard.Core.Corpus.Json
{
    public class JsonInlineImageData
    {
        public string Token => "IMAGE";

        public JsonInlineImageData() {}

        public JsonInlineImageData(ushort imageIndex, ushort imageSymbolStart)
        {
            ImageIndex = imageIndex;
            ImageSymbolStart = imageSymbolStart;
        }

        public ushort ImageIndex { get; set; }
        public ushort ImageSymbolStart { get; set; }
    }
}