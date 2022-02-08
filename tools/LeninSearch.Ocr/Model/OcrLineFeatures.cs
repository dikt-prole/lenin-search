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

        [JsonProperty("li")]
        public double LeftIndent { get; set; }

        [JsonProperty("ri")]
        public double RightIndent { get; set; }

        [JsonProperty("bi")]
        public double BottomIndent { get; set; }

        [JsonProperty("ti")]
        public double TopIndent { get; set; }

        [JsonProperty("btd")]
        public double BelowTopDivider { get; set; }

        [JsonProperty("abd")]
        public double AboveBottomDivider { get; set; }

        [JsonProperty("pps")]
        public double PixelsPerSymbol { get; set; }

        [JsonProperty("w")]
        public double Width { get; set; }

        [JsonProperty("h")]
        public double Height { get; set; }

        [JsonProperty("whr")]
        public double WidthToHeightRatio { get; set; }

        [JsonProperty("wc")]
        public double WordCount { get; set; }

        [JsonProperty("sc")]
        public double SymbolCount { get; set; }

        [JsonProperty("syc")]
        public double SameYCount { get; set; }

        [JsonProperty("ii")]
        public double ImageIndex { get; set; }

        [JsonProperty("swc")]
        public double StartsWithCapital { get; set; }

        [JsonProperty("ews")]
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
            var lineText = line.Words?.Any() == true
                ? string.Join(" ", line.Words.Select(w => w.Text))
                : null;
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
                PixelsPerSymbol = lineText == null
                    ? 0
                    : 1.0 * (line.BottomRightX - line.TopLeftX) / lineText.Length,
                WordCount = line.Words?.Count ?? 0,
                SymbolCount = lineText?.Length ?? 0,
                StartsWithCapital = lineText == null 
                    ? 0 
                    : char.IsUpper(lineText[0]) ? 1 : 0,
                EndsWithSymbol = lineText == null
                    ? 0
                    : Symbols.Contains(lineText.Last()) ? 1 : 0,

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