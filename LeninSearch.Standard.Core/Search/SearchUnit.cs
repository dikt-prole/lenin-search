using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchUnit
    {
        public const int MaxTitleLength = 50;
        public const int MaxPreviewLength = 140;
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
        }

        public void AddChain(WordIndexChain chain)
        {
            WordIndexChains.Add(chain);
        }

        public ushort ParagraphIndex { get; set; }
        public List<WordIndexChain> WordIndexChains { get; set; }
        public string Preview { get; set; }
        public string Title { get; set; }
        public ushort Priority { get; set; }

        public void SetPreviewAndTitle(LsiData lsiData, CorpusFileItem corpusFileItem, LsDictionary dictionary)
        {
            var lsiParagraph = lsiData.Paragraphs[ParagraphIndex];
            var spans = lsiParagraph.GetSpans(this).SkipWhile(s => s.Type != LsiSpanType.SearchResult).ToList();

            Preview = string.Join(' ', spans.Select(s => s.GetText(dictionary.Words)));
            if (Preview.Length > MaxPreviewLength)
            {
                Preview = Preview.Substring(0, MaxPreviewLength);
                var lastSpaceIndex = Preview.LastIndexOf(' ');
                if (lastSpaceIndex != -1)
                {
                    Preview = Preview.Substring(0, lastSpaceIndex);
                }
            }

            var sb = new StringBuilder();
            sb.Append(corpusFileItem.Name);
            if (lsiData.Paragraphs.ContainsKey(ParagraphIndex))
            {
                var headingsHierarchy = lsiData.GetHeadingsDownToZero(ParagraphIndex);
                foreach (var heading in headingsHierarchy)
                {
                    var headingText = heading.GetText(dictionary.Words);
                    if (sb.Length + headingText.Length > MaxTitleLength) break;
                    sb.Append(" > ");
                    sb.Append(headingText);
                }
            }
            Title = sb.ToString();
        }
    }
}