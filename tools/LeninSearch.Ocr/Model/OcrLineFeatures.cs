using System.Collections.Generic;
using System.Drawing.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using Emgu.CV.Aruco;

namespace LeninSearch.Ocr.Model
{
    public class OcrLineFeatures
    {
        private static readonly char[] Symbols = new[] {'.', '?', '!', ':'};

        public double LeftIndent { get; set; }
        public double RightIndent { get; set; }
        public double BottomIndent { get; set; }
        public double TopIndent { get; set; }
        public double BelowTopDivider { get; set; }
        public double AboveBottomDivider { get; set; }
        public double PixelsPerSymbol { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double WidthToHeightRatio { get; set; }
        public double WordCount { get; set; }
        public double SymbolCount { get; set; }
        public double SameYCount { get; set; }
        public double ImageIndex { get; set; }
        public double StartsWithCapital { get; set; }
        public double EndsWithSymbol { get; set; }

        public double[] ToFeatureRow(Dictionary<string, bool> rowModel)
        {
            var props = typeof(OcrLineFeatures).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var rowValues = new List<double>();
            foreach (var propName in rowModel.Keys)
            {
                if (!rowModel[propName]) continue;

                var prop = props.First(p => p.Name == propName);

                rowValues.Add((double)prop.GetValue(this));
            }

            return rowValues.ToArray();
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
                EndsWithSymbol = Symbols.Contains(lastChar) ? 1 : 0,

                // other features
                ImageIndex = page.ImageIndex
            };
        }

        public static Dictionary<string, bool> GetDefaultRowModel()
        {
            var props = typeof(OcrLineFeatures).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var rowModel = props.ToDictionary(p => p.Name, p => true);

            rowModel[nameof(EndsWithSymbol)] = false;
            rowModel[nameof(PixelsPerSymbol)] = false;

            return rowModel;
        }
    }
}