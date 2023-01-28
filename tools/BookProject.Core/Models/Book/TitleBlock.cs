using System.Drawing;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class TitleBlock : Block
    {
        [JsonProperty("lvl")]
        public int Level { get; set; }

        [JsonProperty("txt")]
        public string? Text { get; set; }

        public static TitleBlock FromRectangle(Rectangle rect)
        {
            return new TitleBlock
            {
                TopLeftX = rect.X,
                TopLeftY = rect.Y,
                BottomRightX = rect.X + rect.Width,
                BottomRightY = rect.Y + rect.Height
            };
        }
    }
}