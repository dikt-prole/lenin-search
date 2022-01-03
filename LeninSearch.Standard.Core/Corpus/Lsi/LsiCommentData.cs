namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiCommentData
    {
        public ushort CommentIndex { get; set; }
        public ushort WordPosition { get; set; }
        public uint[] WordIndexes { get; set; }
    }
}