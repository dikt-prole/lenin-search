using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsIndexData
    {
        public Dictionary<uint, List<LsWordParagraphData>> WordParagraphData { get; set; }
        public List<LsWordHeadingData> HeadingData { get; set; }
        public List<LsPageData> PageData { get; set; }
        public Dictionary<ushort, ushort> VideoOffsets { get; set; }
        public List<LsiVideoDataItem> VideoData { get; set; }
        public Dictionary<ushort, ushort> ImageData { get; set; }
        public Dictionary<ushort, List<LsiMarkupData>> Markups { get; set; }
        public Dictionary<ushort, List<LsiCommentData>> Comments { get; set; }

        private LsData _lsData;
        public LsData LsData => _lsData ??= ToLsData();
        public LsData ToLsData()
        {
            var lsData = new LsData
            {
                Paragraphs = new Dictionary<ushort, LsParagraph>(),
                Headings = new List<LsHeading>(),
                Pages = new Dictionary<ushort, ushort>()
            };

            foreach (var wordIndex in WordParagraphData.Keys)
            {
                foreach (var wpData in WordParagraphData[wordIndex])
                {
                    if (!lsData.Paragraphs.ContainsKey(wpData.ParagraphIndex))
                    {
                        lsData.Paragraphs.Add(wpData.ParagraphIndex, new LsParagraph(wpData.ParagraphIndex));
                    }

                    AddWord(lsData.Paragraphs[wpData.ParagraphIndex].WordIndexes, wpData, wordIndex);
                }
            }

            foreach (var headingData in HeadingData)
            {
                var paragraph = lsData.Paragraphs[headingData.Index];
                var lsHeading = LsHeading.FromLsParagraph(paragraph, headingData.Level);
                lsData.Headings.Add(lsHeading);
                lsData.Paragraphs[headingData.Index].IsHeading = true;
            }

            foreach (var pageData in PageData)
            {
                lsData.Pages.Add(pageData.Index, pageData.Number);
                if (!lsData.Paragraphs.ContainsKey(pageData.Index))
                {
                    lsData.Paragraphs.Add(pageData.Index, new LsParagraph(pageData.Index));
                }
                lsData.Paragraphs[pageData.Index].IsPageNumber = true;
                lsData.Paragraphs[pageData.Index].PageNumber = pageData.Number;
            }

            return lsData;
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
            var videoDataItem = VideoData.FirstOrDefault(vdi => vdi.FirstParagraphIndex <= paragraphIndex && paragraphIndex <= vdi.LastParagraphIndex);
            return videoDataItem?.VideoId;
        }
    }
}