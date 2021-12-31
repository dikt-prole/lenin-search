using System.Reflection;

namespace LeninSearch.Standard.Core.Corpus
{
    public class JsonCommentData
    {
        public string CommentId => $"c{CommentIndex}";
        public ushort CommentIndex { get; set; }
        public string Text { get; set; }
    }
}