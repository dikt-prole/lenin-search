using System.Drawing;
using Newtonsoft.Json;

namespace LeninSearch.Studio.Core.Models
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

        [JsonProperty("lbd")]
        public double LineBottomDistance { get; set; }

        [JsonProperty("ltd")]
        public double LineTopDistance { get; set; }

        [JsonProperty("cln")]
        public bool IsCommentLinkNumber { get; set; }

        [JsonIgnore]
        public int Width => BottomRightX - TopLeftX;

        [JsonIgnore]
        public int Height => BottomRightY - TopLeftY;

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        [JsonIgnore]
        public int CenterX => (TopLeftX + BottomRightX) / 2;

        [JsonIgnore]
        public int CenterY => (TopLeftY + BottomRightY) / 2;

        public bool IsInsideWordCircle(Point point)
        {
            // dx^2 + dy^2 <= R^2
            return (point.X - CenterX) * (point.X - CenterX) + (point.Y - CenterY) * (point.Y - CenterY) <=
                   OcrSettings.WordCircleRadius * OcrSettings.WordCircleRadius;
        }
    }
}