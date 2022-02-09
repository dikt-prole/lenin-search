using System;
using System.Drawing;
using System.Linq;
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

        [JsonIgnore]
        public int CenterX => (TopLeftX + BottomRightX) / 2;

        [JsonIgnore]
        public int CenterY => (TopLeftY + BottomRightY) / 2;

        public bool ContainsCommentLinkNumbers(int maxLinkNumber)
        {
            if (string.IsNullOrEmpty(Text)) return false;

            var numberChars = Text.Where(char.IsNumber).ToArray();

            if (!numberChars.Any()) return false;

            var number = int.Parse(new string(numberChars));

            return number <= maxLinkNumber;
        } 

        public bool IsInsideWordCircle(Point point)
        {
            // dx^2 + dy^2 <= R^2
            return (point.X - CenterX) * (point.X - CenterX) + (point.Y - CenterY) * (point.Y - CenterY) <=
                   OcrSettings.WordCircleRadius * OcrSettings.WordCircleRadius;
        }
    }
}