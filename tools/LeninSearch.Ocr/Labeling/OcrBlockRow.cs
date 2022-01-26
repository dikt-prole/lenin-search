namespace LeninSearch.Ocr.Labeling
{
    public class OcrBlockRow
    {
        // block id
        public string FileName { get; set; }
        public int BlockIndex { get; set; }

        // block data
        public int BottomIndent { get; set; }
        public int LeftIndent { get; set; }
        public int RightIndent { get; set; }
        public int TopIndent { get; set; }
        public double PixelsPerSymbol { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double WidthToHeightRatio { get; set; }
        public int WordCount { get; set; }
        public int SymbolCount { get; set; }
        public int SameyCount { get; set; }
        public int BottomLineDistance { get; set; }
        public int TopLineDistance { get; set; }

        // block label
        public OcrBlockLabel? Label { get; set; }

        public override string ToString()
        {
            if (Label == null) return $"{FileName}-{BlockIndex}";

            return $"{FileName}-{BlockIndex} ({Label})";
        }
    }
}