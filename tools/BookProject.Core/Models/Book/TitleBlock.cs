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
                BottomRightY = rect.Y + rect.Height,
                State = BlockState.Normal
            };
        }

        public static TitleBlock Copy(TitleBlock proto)
        {
            return new TitleBlock
            {
                Level = proto.Level,
                Text = proto.Text,
                TopLeftX = proto.TopLeftX,
                TopLeftY = proto.TopLeftY,
                BottomRightX = proto.BottomRightX,
                BottomRightY = proto.BottomRightY,
                State = BlockState.Normal
            };
        }
    }
}