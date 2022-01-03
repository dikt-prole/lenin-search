using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiSpan
    {
        public ushort ImageIndex { get; set; }
        public ushort CommentIndex { get; set; }
        public List<uint> WordIndexes { get; set; }
        public LsiSpanType Type { get; set; }

        public string GetText(string[] dictionary)
        {
            if (Type == LsiSpanType.Comment) return $"[К{CommentIndex}]";

            var words = WordIndexes.Select(wi => dictionary[wi]).ToList();

            return TextUtil.GetParagraph(words);
        }

        public static LsiSpan Plain()
        {
            return new LsiSpan
            {
                Type = LsiSpanType.Plain,
                WordIndexes = new List<uint>()
            };
        }

        public static LsiSpan Strong()
        {
            return new LsiSpan
            {
                Type = LsiSpanType.Strong,
                WordIndexes = new List<uint>()
            };
        }

        public static LsiSpan Emphasis()
        {
            return new LsiSpan
            {
                Type = LsiSpanType.Emphasis,
                WordIndexes = new List<uint>()
            };
        }

        public static LsiSpan InlineImage(ushort imageIndex)
        {
            return new LsiSpan
            {
                Type = LsiSpanType.InlineImage,
                ImageIndex = imageIndex
            };
        }

        public static LsiSpan Comment(ushort commentIndex)
        {
            return new LsiSpan
            {
                Type = LsiSpanType.Comment,
                CommentIndex = commentIndex
            };
        }

        public static LsiSpan SearchResult()
        {
            return new LsiSpan
            {
                Type = LsiSpanType.SearchResult,
                WordIndexes = new List<uint>()
            };
        }
    }

    public enum LsiSpanType
    {
        Strong, Emphasis, Plain, InlineImage, Comment, SearchResult
    }
}