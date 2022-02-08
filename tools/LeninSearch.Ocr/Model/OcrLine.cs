using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
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
    }
}