using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiHeadingTree
    {
        public List<LsiHeadingLeaf> Children { get; set; }
    }

    public class LsiHeadingLeaf : LsiParagraph
    {
        public List<LsiHeadingLeaf> Children { get; set; }

        public LsiHeadingLeaf(ushort index) : base(index)
        { }

        public LsiHeadingLeaf(LsiParagraph p) : base(p.Index)
        {
            WordIndexes = p.WordIndexes;
            HeadingLevel = p.HeadingLevel;
            Index = p.Index;
            Comments = p.Comments;
            InlineImages = p.InlineImages;
            Markups = p.Markups;
            ImageIndex = p.ImageIndex;
            PageNumber = p.PageNumber;
            Children = new List<LsiHeadingLeaf>();
        }
    }
}