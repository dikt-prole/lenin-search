using System.Drawing;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrImageBlock
    {
        private const int DragPointSize = 20;

        [JsonProperty("tlx")]
        public int TopLeftX { get; set; }

        [JsonProperty("tly")]
        public int TopLeftY { get; set; }

        [JsonProperty("brx")]
        public int BottomRightX { get; set; }

        [JsonProperty("bry")]
        public int BottomRightY { get; set; }

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        [JsonIgnore]
        public Rectangle TopLeftRectangle => new Rectangle(TopLeftX - DragPointSize / 2, TopLeftY - DragPointSize / 2, DragPointSize, DragPointSize);

        [JsonIgnore]
        public Rectangle BottomRightRectangle => new Rectangle(BottomRightX - DragPointSize / 2, BottomRightY - DragPointSize / 2, DragPointSize, DragPointSize);
    }
}