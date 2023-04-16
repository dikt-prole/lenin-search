using System.Collections.Generic;

namespace LenLib.Standard.Core.Corpus.Json
{
    public class JsonParagraph
    {
        public string Text { get; set; }
        public ushort OffsetSeconds { get; set; }
        public string VideoId { get; set; }
        public JsonParagraphType ParagraphType { get; set; }
        public ushort? ImageIndex { get; set; }
        public List<JsonCommentData> Comments { get; set; }
        public List<JsonMarkupData> Markups { get; set; }
        public List<JsonInlineImageData> InlineImages { get; set; }
    }
}