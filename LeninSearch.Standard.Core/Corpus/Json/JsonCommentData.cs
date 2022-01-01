namespace LeninSearch.Standard.Core.Corpus.Json
{
    public class JsonCommentData
    {
        public JsonCommentData() {}

        public JsonCommentData(string commentId, ushort commentIndex, ushort commentSymbolStart)
        {
            CommentId = commentId;
            CommentIndex = commentIndex;
            CommentSymbolStart = commentSymbolStart;
        }

        public ushort CommentIndex { get; set; }
        public ushort CommentSymbolStart { get; set; }
        public string CommentId { get; set; }
        public string Text { get; set; }
    }
}