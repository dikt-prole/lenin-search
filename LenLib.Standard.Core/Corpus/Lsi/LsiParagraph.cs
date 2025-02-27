﻿using System.Collections.Generic;
using System.Linq;
using LenLib.Standard.Core.Corpus.Json;
using LenLib.Standard.Core.Search;

namespace LenLib.Standard.Core.Corpus.Lsi
{
    public class LsiParagraph
    {
        public LsiParagraph(ushort index)
        {
            Index = index;
            WordIndexes = new List<uint>();
            HeadingLevel = byte.MaxValue;
            PageNumber = ushort.MaxValue;
            ImageIndex = ushort.MaxValue;
            InlineImages = new List<LsiInlineImageData>();
            Markups = new List<LsiMarkupData>();
            Comments = new List<LsiCommentData>();
        }

        public ushort Index { get; set; }
        public List<uint> WordIndexes { get; set; }
        public string GetText(string[] dictionary)
        {
            var spans = GetSpans(null).Where(s => s.Type != LsiSpanType.Comment).ToList();

            return string.Join(" ", spans.Select(s => s.GetText(dictionary)));
        }
        public ushort PageNumber { get; set; }
        public bool IsPageNumber => PageNumber < ushort.MaxValue;
        public byte HeadingLevel { get; set; }
        public bool IsHeading => HeadingLevel < byte.MaxValue;
        public ushort ImageIndex { get; set; }
        public bool IsImage => ImageIndex < ushort.MaxValue;
        public List<LsiInlineImageData> InlineImages { get; set; }
        public bool HasInlineImages => InlineImages.Any();
        public List<LsiMarkupData> Markups { get; set; }
        public bool HasMarkups => Markups.Any();
        public List<LsiCommentData> Comments { get; set; }
        public bool HasComments => Comments.Any();

        public List<LsiSpan> GetSpans(SearchUnit searchUnit)
        {
            if (searchUnit?.ParagraphIndex != Index)
            {
                searchUnit = null;
            }

            var spans = new List<LsiSpan>();

            if (WordIndexes.Count == 0) return spans;

            ushort wordPosition = 0;
            while (true)
            {
                if (wordPosition >= WordIndexes.Count) break;
                var wordData = GetWordData(wordPosition, searchUnit);
                var span = ConstructSpan(wordData.SpanType, wordData.CommentIndex, wordData.InlineImageIndex);
                spans.Add(span);
                var wordIndex = WordIndexes[wordPosition];
                wordPosition++;
                if (span.WordIndexes != null)
                {
                    span.WordIndexes.Add(wordIndex);
                    break;
                }
            }

            for (; wordPosition < WordIndexes.Count; wordPosition++)
            {
                var wordData = GetWordData(wordPosition, searchUnit);
                var lastSpan = spans.Last();

                if (lastSpan.Type == wordData.SpanType && lastSpan.WordIndexes != null)
                {
                    lastSpan.WordIndexes.Add(WordIndexes[wordPosition]);
                    continue;
                }

                var span = ConstructSpan(wordData.SpanType, wordData.CommentIndex, wordData.InlineImageIndex);
                if (span.Type != LsiSpanType.Comment && span.Type != LsiSpanType.InlineImage)
                {
                    span.WordIndexes.Add(WordIndexes[wordPosition]);
                }

                spans.Add(span);
            }

            return spans;
        }

        private (LsiSpanType SpanType, ushort InlineImageIndex, ushort CommentIndex) GetWordData(ushort wordPosition, SearchUnit searchUnit)
        {
            if (searchUnit?.WordIndexChains[0].WordIndexes.Contains(WordIndexes[wordPosition]) == true)
            {
                return (LsiSpanType.SearchResult, 0, 0);
            }

            var markup = Markups.FirstOrDefault(m => m.WordPosition <= wordPosition && wordPosition < m.WordPosition + m.WordLength);

            if (markup?.MarkupType == MarkupType.Emphasis) return (LsiSpanType.Emphasis, 0, 0);

            if (markup?.MarkupType == MarkupType.Strong) return (LsiSpanType.Strong, 0, 0);

            var comment = Comments.FirstOrDefault(c => c.WordPosition == wordPosition);

            if (comment != null) return (LsiSpanType.Comment, 0, comment.CommentIndex);

            var inlineImage = InlineImages.FirstOrDefault(i => i.WordPosition == wordPosition);

            if (inlineImage != null) return (LsiSpanType.InlineImage, inlineImage.ImageIndex, 0);

            return (LsiSpanType.Plain, 0, 0);
        }

        private LsiSpan ConstructSpan(LsiSpanType spanType, ushort commentIndex, ushort inlineImageIndex)
        {
            switch (spanType)
            {
                case LsiSpanType.SearchResult:
                    return LsiSpan.SearchResult();
                case LsiSpanType.Comment:
                    return LsiSpan.Comment(commentIndex);
                case LsiSpanType.InlineImage:
                    return LsiSpan.InlineImage(inlineImageIndex);
                case LsiSpanType.Strong:
                    return LsiSpan.Strong();
                case LsiSpanType.Emphasis:
                    return LsiSpan.Emphasis();
            }

            return LsiSpan.Plain();
        }
    }
}