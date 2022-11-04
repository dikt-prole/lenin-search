using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;
using Newtonsoft.Json;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchUnit
    {
        public const int MaxTitleLength = 100;
        public SearchUnit() {}

        public SearchUnit(ushort paragraphIndex)
        {
            ParagraphIndex = paragraphIndex;
            WordIndexChains = new List<WordIndexChain>();
        }

        public SearchUnit(ushort paragraphIndex, List<WordData> wordDatas)
        {
            ParagraphIndex = paragraphIndex;
            WordIndexChains = new List<WordIndexChain>
            {
                new WordIndexChain
                {
                    WordIndexes = wordDatas.Select(w => w.WordIndex).ToList()
                }
            };

            if (wordDatas.Count == 0)
            {
                MatchSpanLength = 0;
            }
            else if (wordDatas.Count == 1)
            {
                MatchSpanLength = 1;
            }
            else
            {
                var wordPositions = wordDatas.Select(w => w.WordPosition).ToList();
                MatchSpanLength = (ushort)(wordPositions.Max() - wordPositions.Min());
            }
        }

        public void AddChain(WordIndexChain chain)
        {
            WordIndexChains.Add(chain);
        }

        public ushort ParagraphIndex { get; set; }
        public List<WordIndexChain> WordIndexChains { get; set; }
        public string Preview { get; set; }
        public string Title { get; set; }
        public ushort MatchSpanLength { get; set; }

        public void SetPreviewAndTitle(LsiData lsiData, CorpusFileItem corpusFileItem, LsDictionary dictionary)
        {
            var lsiParagraph = lsiData.Paragraphs[ParagraphIndex];
            
            var isHeading = lsiData.HeadingParagraphs.Any(h => h.Index == ParagraphIndex);

            if (isHeading)
            {
                Preview = lsiParagraph.GetText(dictionary.Words);
            }
            else
            {
                var spans = lsiParagraph.GetSpans(this);
                var beforeSearchMatchSpans = spans.TakeWhile(s => s.Type != LsiSpanType.SearchResult).ToList();
                var beforeSearchMatchText = string.Join(' ', beforeSearchMatchSpans.Select(s => s.GetText(dictionary.Words)));
                var afterSearchMatchText = string.Join(' ', spans.Except(beforeSearchMatchSpans).Select(s => s.GetText(dictionary.Words)));

                var beforeSearchMatchTextSentenceEndIndex = beforeSearchMatchText.LastIndexOfAny(new []{'.', '?', '!'});
                if (beforeSearchMatchTextSentenceEndIndex != -1)
                {
                    beforeSearchMatchText = beforeSearchMatchText.Substring(beforeSearchMatchTextSentenceEndIndex);
                }

                var afterSearchMatchTextSentenceEndIndex = afterSearchMatchText.IndexOfAny(new[] { '.', '?', '!' });
                if (afterSearchMatchTextSentenceEndIndex != -1)
                {
                    afterSearchMatchText = afterSearchMatchText.Substring(0, afterSearchMatchTextSentenceEndIndex);
                }

                Preview = $"{beforeSearchMatchText} {afterSearchMatchText}".TrimStart('.', '?', '!', ' ');
            }

            var sb = new StringBuilder();
            sb.Append(corpusFileItem.Name);
            if (!isHeading)
            {
                var headingsHierarchy = lsiData.GetHeadingsDownToZero(ParagraphIndex);
                var titleTokens =
                    new[] { corpusFileItem.Name }.Concat(headingsHierarchy.Select(h => h.GetText(dictionary.Words)));
                Title = string.Join(" > ", titleTokens);
                if (Title.Length > MaxTitleLength)
                {
                    Title = Title.Substring(0, MaxTitleLength);
                }
            }
            else
            {
                Title = corpusFileItem.Name;
            }
        }
    }
}