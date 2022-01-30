using LeninSearch.Ocr.Labeling;

namespace LeninSearch.Ocr.Model
{
    public class OcrFeaturedBlock
    {
        public string FileName { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public string Text { get; set; }
        public OcrBlockLabel? Label { get; set; }
        public OcrBlockFeatures Features { get; set; }
        public int BlockIndex { get; set; }
    }
}