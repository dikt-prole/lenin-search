namespace LeninSearch.Standard.Core.Corpus
{
    public class Paragraph
    {
        public string Text { get; set; }
        public bool Centered { get; set; }
        public int LeftIndent { get; set; }
        public bool Bold { get; set; }
        public ushort OffsetSeconds { get; set; }
        public string VideoId { get; set; }
        public ParagraphType ParagraphType { get; set; }
    }
}