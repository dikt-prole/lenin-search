using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrLineFeatures
    {
        public int LeftIndent { get; set; }
        public int RightIndent { get; set; }
        public int BottomIndent { get; set; }
        public int TopIndent { get; set; }
        public int BelowTopDivider { get; set; }
        public int AboveBottomDivider { get; set; }
        public double PixelsPerSymbol { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double WidthToHeightRatio { get; set; }
        public int WordCount { get; set; }
        public int SymbolCount { get; set; }
        public int SameYCount { get; set; }
        public int ImageIndex { get; set; }
        public int StartsWithCapital { get; set; }
        public int EndsWithSymbol { get; set; }

        public double[] ToFeatureRow()
        {
            return new double[]
            {
                LeftIndent,
                RightIndent,
                BottomIndent,
                TopIndent,
                BelowTopDivider,
                AboveBottomDivider,
                Width,
                Height,
                WidthToHeightRatio,

                PixelsPerSymbol,
                WordCount,
                SymbolCount,
                SameYCount,
                StartsWithCapital,
                EndsWithSymbol,

                ImageIndex
            };
        }

        public OcrLineFeatures Copy()
        {
            return JsonConvert.DeserializeObject<OcrLineFeatures>(JsonConvert.SerializeObject(this));
        }
    }
}