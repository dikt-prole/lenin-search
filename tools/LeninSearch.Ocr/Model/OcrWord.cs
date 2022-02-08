using System;
using System.Drawing;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrWord
    {
        [JsonProperty("tlx")]
        public int TopLeftX { get; set; }

        [JsonProperty("tly")]
        public int TopLeftY { get; set; }

        [JsonProperty("brx")]
        public int BottomRightX { get; set; }

        [JsonProperty("bry")]
        public int BottomRightY { get; set; }

        [JsonProperty("txt")]
        public string Text { get; set; }

        [JsonProperty("itl")]
        public bool Italic { get; set; }

        [JsonProperty("bld")]
        public bool Bold { get; set; }

        [JsonProperty("cli")]
        public int? CommentLineIndex { get; set; }

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);
    }
}