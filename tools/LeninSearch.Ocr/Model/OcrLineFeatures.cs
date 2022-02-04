using Newtonsoft.Json;
using System.Linq;

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

        public static OcrLineFeatures Calculate(OcrPage page, OcrLine line)
        {
            var lineRectangle = line.Rectangle;
            var pageWideRectangle = line.PageWideRectangle(page.Width);
            var lineText = string.Join(" ", line.Words.Select(w => w.Text));
            var lastChar = lineText.Last();
            return new OcrLineFeatures
            {
                // geometric features
                LeftIndent = line.TopLeftY,
                RightIndent = page.Width - line.BottomRightX,
                TopIndent = line.TopLeftY,
                BottomIndent = page.Height - line.BottomRightY,
                BelowTopDivider = page.TopDivider.Y < line.TopLeftY ? 1 : 0,
                AboveBottomDivider = page.BottomDivider.Y > line.TopLeftY ? 1 : 0,
                Width = lineRectangle.Width,
                Height = lineRectangle.Height,
                SameYCount = page.Lines.Count(pl =>
                    pl != line && pageWideRectangle.IntersectsWith(pl.PageWideRectangle(page.Width))),
                WidthToHeightRatio = 1.0 * lineRectangle.Width / lineRectangle.Height,

                // text features
                PixelsPerSymbol = 1.0 * (line.BottomRightX - line.TopLeftX) / lineText.Length,
                WordCount = line.Words.Count,
                SymbolCount = lineText.Length,
                StartsWithCapital = char.IsUpper(lineText[0]) ? 1 : 0,
                EndsWithSymbol = char.IsSymbol(lastChar) ? 1 : 0,

                // other features
                ImageIndex = page.ImageIndex
            };
        }
    }
}