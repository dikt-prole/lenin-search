using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrBlockFeatures
    {
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
        public int SameYLevelBlockCount { get; set; }
        public int BottomLineDistance { get; set; }
        public int TopLineDistance { get; set; }
        public int ImageIndex { get; set; }
        public int FirstLineIndent { get; set; }

        public double[] ToFeatureRow()
        {
            return new double[]
            {
                BottomIndent,
                LeftIndent,
                RightIndent,
                TopIndent,
                PixelsPerSymbol,
                Width,
                Height,
                WidthToHeightRatio,
                WordCount,
                SymbolCount,
                SameYLevelBlockCount,
                BottomLineDistance,
                TopLineDistance,
                ImageIndex,
                FirstLineIndent
            };
        }

        public OcrBlockFeatures Copy()
        {
            return JsonConvert.DeserializeObject<OcrBlockFeatures>(JsonConvert.SerializeObject(this));
        }
    }
}