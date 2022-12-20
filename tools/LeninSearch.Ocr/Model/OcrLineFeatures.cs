using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;

namespace LeninSearch.Ocr.Model
{
    public class OcrLineFeatures
    {
        [JsonProperty("li")]
        public double LeftIndent { get; set; }

        [JsonProperty("ri")]
        public double RightIndent { get; set; }

        [JsonProperty("btd")]
        public double BelowTopDivider { get; set; }

        [JsonProperty("abd")]
        public double AboveBottomDivider { get; set; }

        [JsonProperty("w")]
        public double Width { get; set; }

        [JsonProperty("swc")]
        public double StartsWithCapital { get; set; }

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

        public double[] ToFeatureRow()
        {
            var rowValues = new double[]
            {
                LeftIndent,
                RightIndent,
                Width,
                StartsWithCapital
            };

            return rowValues;
        }

        public static OcrLineFeatures Calculate(OcrPage page, OcrLine line)
        {
            var lineRectangle = line.Rectangle;
            var lineText = line.Words?.Any(w => !string.IsNullOrEmpty(w.Text)) == true
                ? string.Join(" ", line.Words.Select(w => w.Text))
                : null;

            return new OcrLineFeatures
            {
                // geometric features
                LeftIndent = line.TopLeftY,
                RightIndent = page.Width - line.BottomRightX,
                BelowTopDivider = page.TopDivider.Y < line.TopLeftY ? 1 : 0,
                AboveBottomDivider = page.BottomDivider.Y > line.TopLeftY ? 1 : 0,
                Width = lineRectangle.Width,

                // text features
                StartsWithCapital = string.IsNullOrEmpty(lineText)
                    ? 0 
                    : char.IsUpper(lineText[0]) ? 1 : 0
            };
        }

        public static Dictionary<string, bool> GetDefaultRowModel()
        {
            var props = typeof(OcrLineFeatures).GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var rowModel = props.ToDictionary(p => p.Name, p => true);

            return rowModel;
        }
    }
}