using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace LeninSearch.Studio.WinForms.Model
{
    public class OcrLine
    {
        [JsonProperty("li")]
        public int LineIndex { get; set; }

        [JsonProperty("tlx")]
        public int TopLeftX { get; set; }

        [JsonProperty("tly")]
        public int TopLeftY { get; set; }

        [JsonProperty("brx")]
        public int BottomRightX { get; set; }

        [JsonProperty("bry")]
        public int BottomRightY { get; set; }

        [JsonProperty("fts")]
        public OcrLineFeatures Features { get; set; }

        [JsonProperty("lbl")]
        public OcrLabel? Label { get; set; }

        [JsonProperty("wds")]
        public List<OcrWord> Words { get; set; }

        [JsonProperty("dst")]
        public bool DisplayText { get; set; }

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        public Rectangle PageWideRectangle(int pageWidth) => new Rectangle(0, TopLeftY, pageWidth, BottomRightY - TopLeftY);

        [JsonIgnore]
        public string TextPreview => Words?.Any() == true ? string.Join(" ", Words.Select(w => w.Text)) : null;

        public override string ToString()
        {
            return $"[{Label}]: {TextPreview}";
        }

        public void FitRectangleToWords()
        {
            if (Words?.Any() != true) return;

            TopLeftX = Words.Min(w => w.TopLeftX);
            TopLeftY = Words.Min(w => w.TopLeftY);
            BottomRightX = Words.Max(w => w.BottomRightX);
            BottomRightY = Words.Max(w => w.BottomRightY);
        }
    }
}