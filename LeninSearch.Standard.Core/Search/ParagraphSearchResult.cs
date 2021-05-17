namespace LeninSearch.Standard.Core.Search
{
    public class ParagraphSearchResult
    {
        public ParagraphSearchResult(ushort paragraphIndex, WordIndexChain wordIndexChain)
        {
            ParagraphIndex = paragraphIndex;
            WordIndexChain = wordIndexChain;
        }

        public ushort ParagraphIndex { get; set; }
        public WordIndexChain WordIndexChain { get; set; }
    }
}