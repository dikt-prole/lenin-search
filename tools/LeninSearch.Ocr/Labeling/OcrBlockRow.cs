namespace LeninSearch.Ocr.Labeling
{
    public class OcrBlockRow
    {
        // block id
        public string ImageFile { get; set; }
        public string OcrJsonFile { get; set; }
        public int BlockIndex { get; set; }

        // block data
        public int BottomIndent { get; set; }
        public int LeftIndent { get; set; }
        public int RightIndent { get; set; }
        public int TopIndent { get; set; }
        public double PixelsPerSymbol { get; set; }

        // block label
        public OcrBlockLabel? Label { get; set; }
    }
}