using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiData
    {
        public Dictionary<uint, List<LsWordParagraphData>> WordParagraphData { get; set; }
        public List<LsWordHeadingData> Headings { get; set; }
        public List<LsPageData> Pages { get; set; }
        public Dictionary<ushort, ushort> Offsets { get; set; }
        public List<LsiVideoDataItem> Videos { get; set; }
        public Dictionary<ushort, ushort> Images { get; set; }
        public Dictionary<ushort, List<LsiMarkupData>> Markups { get; set; }
        public Dictionary<ushort, List<LsiCommentData>> Comments { get; set; }
        public Dictionary<ushort, List<LsiInlineImageData>> InlineImages { get; set; }

        private List<LsiParagraph> _headingParagraphs;
        public List<LsiParagraph> HeadingParagraphs
        {
            get
            {
                return _headingParagraphs ??= Paragraphs.Select(p => p.Value).Where(p => p.IsHeading).OrderBy(p => p.Index).ToList();
            }
        }

        private Dictionary<ushort, LsiParagraph> _paragraphs;
        public Dictionary<ushort, LsiParagraph> Paragraphs
        {
            get
            {
                if (_paragraphs != null) return _paragraphs;

                _paragraphs = new Dictionary<ushort, LsiParagraph>();

                foreach (var wordIndex in WordParagraphData.Keys)
                {
                    foreach (var wpData in WordParagraphData[wordIndex])
                    {
                        if (!_paragraphs.ContainsKey(wpData.ParagraphIndex))
                        {
                            _paragraphs.Add(wpData.ParagraphIndex, new LsiParagraph(wpData.ParagraphIndex));
                        }

                        AddWord(_paragraphs[wpData.ParagraphIndex].WordIndexes, wpData, wordIndex);
                    }
                }

                foreach (var heading in Headings)
                {
                    var paragraph = _paragraphs[heading.Index];
                    paragraph.HeadingLevel = heading.Level;
                }

                foreach (var pageData in Pages)
                {
                    if (!_paragraphs.ContainsKey(pageData.Index))
                    {
                        _paragraphs.Add(pageData.Index, new LsiParagraph(pageData.Index));
                    }
                    _paragraphs[pageData.Index].PageNumber = pageData.Number;
                }

                foreach (var paragraphIndex in Images.Keys)
                {
                    _paragraphs[paragraphIndex].ImageIndex = Images[paragraphIndex];
                }

                foreach (var paragraphIndex in InlineImages.Keys)
                {
                    _paragraphs[paragraphIndex].InlineImages = InlineImages[paragraphIndex];
                }

                foreach (var paragraphIndex in Markups.Keys)
                {
                    _paragraphs[paragraphIndex].Markups = Markups[paragraphIndex];
                }

                foreach (var paragraphIndex in Comments.Keys)
                {
                    _paragraphs[paragraphIndex].Comments = Comments[paragraphIndex];
                }

                return _paragraphs;
            }
        }

        private void AddWord(List<uint> paragraphWords, LsWordParagraphData wpData, uint wordIndex)
        {
            while (paragraphWords.Count <= wpData.WordPosition)
            {
                paragraphWords.Add(0);
            }

            paragraphWords[wpData.WordPosition] = wordIndex;
        }

        public string GetVideoId(ushort paragraphIndex)
        {
            var videoDataItem = Videos.FirstOrDefault(vdi => vdi.FirstParagraphIndex <= paragraphIndex && paragraphIndex <= vdi.LastParagraphIndex);
            return videoDataItem?.VideoId;
        }
    }
}