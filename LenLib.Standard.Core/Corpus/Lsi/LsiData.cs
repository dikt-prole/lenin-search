using System.Collections.Generic;
using System.Linq;

namespace LenLib.Standard.Core.Corpus.Lsi
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

                if (Images?.Any() == true)
                {
                    foreach (var paragraphIndex in Images.Keys)
                    {
                        _paragraphs[paragraphIndex].ImageIndex = Images[paragraphIndex];
                    }
                }

                if (InlineImages?.Any() == true)
                {
                    foreach (var paragraphIndex in InlineImages.Keys)
                    {
                        if (_paragraphs.ContainsKey(paragraphIndex))
                        {
                            _paragraphs[paragraphIndex].InlineImages = InlineImages[paragraphIndex];
                        }
                    }
                }

                if (Markups?.Any() == true)
                {
                    foreach (var paragraphIndex in Markups.Keys)
                    {
                        if (_paragraphs.ContainsKey(paragraphIndex))
                        {
                            _paragraphs[paragraphIndex].Markups = Markups[paragraphIndex];
                        }
                    }
                }

                if (Comments?.Any() == true)
                {
                    foreach (var paragraphIndex in Comments.Keys)
                    {
                        if (_paragraphs.ContainsKey(paragraphIndex))
                        {
                            _paragraphs[paragraphIndex].Comments = Comments[paragraphIndex];
                        }
                    }
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

        private LsiHeadingTree _headingTree;
        public LsiHeadingTree HeadingTree
        {
            get
            {
                if (_headingTree != null) return _headingTree;

                _headingTree = new LsiHeadingTree {Children = new List<LsiHeadingLeaf>()};

                var headingParagraphs = HeadingParagraphs.OrderByDescending(hp => hp.Index);

                var leafs = new List<LsiHeadingLeaf>();

                foreach (var hp in headingParagraphs)
                {
                    var lowerLeafs = leafs.Where(l => l.HeadingLevel > hp.HeadingLevel).OrderBy(l => l.Index).ToList();
                    leafs = leafs.Except(lowerLeafs).ToList();
                    leafs.Add(new LsiHeadingLeaf(hp) {Children = lowerLeafs});
                }

                _headingTree.Children = leafs.OrderBy(l => l.Index).ToList();

                return _headingTree;
            }
        }
    }
}