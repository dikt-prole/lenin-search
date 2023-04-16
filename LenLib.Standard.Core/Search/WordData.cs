namespace LenLib.Standard.Core.Search
{
    public class WordData
    {
        public WordData(uint wordIndex, ushort paragraphIndex, ushort wordPosition)
        {
            ParagraphIndex = paragraphIndex;
            WordIndex = wordIndex;
            WordPosition = wordPosition;
        }
        public uint WordIndex { get; }
        public ushort ParagraphIndex { get; }
        public ushort WordPosition { get; }
    }
}