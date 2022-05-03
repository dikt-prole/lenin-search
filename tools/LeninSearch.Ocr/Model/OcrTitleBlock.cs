using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrTitleBlock
    {
        private const int DragPointSize = 20;

        [JsonProperty("tll")]
        public int TitleLevel { get; set; }

        [JsonProperty("tlt")]
        public string TitleText { get; set; }

        [JsonProperty("tlx")]
        public int TopLeftX { get; set; }

        [JsonProperty("tly")]
        public int TopLeftY { get; set; }

        [JsonProperty("brx")]
        public int BottomRightX { get; set; }

        [JsonProperty("bry")]
        public int BottomRightY { get; set; }

        [JsonProperty("ctl")]
        public List<OcrWord> CommentLinks { get; set; }

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        [JsonIgnore]
        public Rectangle TopLeftRectangle => new Rectangle(TopLeftX - DragPointSize / 2, TopLeftY - DragPointSize / 2, DragPointSize, DragPointSize);

        [JsonIgnore]
        public Rectangle BottomRightRectangle => new Rectangle(BottomRightX - DragPointSize / 2, BottomRightY - DragPointSize / 2, DragPointSize, DragPointSize);
    }
}