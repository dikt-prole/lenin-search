using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Corpus
{
    public class Paragraph
    {
        public string Text { get; set; }
        public ushort OffsetSeconds { get; set; }
        public string VideoId { get; set; }
        public ParagraphType ParagraphType { get; set; }
        public ushort? ImageIndex { get; set; }
        public List<CommentData> Comments { get; set; }
        public List<MarkupData> Markups { get; set; }
    }
}