using System.Drawing;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class Block
    {
        public const int PbDragPointSize = 10;

        [JsonProperty("tlx")]
        public int TopLeftX { get; set; }

        [JsonProperty("tly")]
        public int TopLeftY { get; set; }

        [JsonProperty("brx")]
        public int BottomRightX { get; set; }

        [JsonProperty("bry")]
        public int BottomRightY { get; set; }

        [JsonProperty("stt")]
        public BlockState State { get; set; }

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        [JsonIgnore]
        public Point TopDragCenter => new Point((TopLeftX + BottomRightX) / 2, TopLeftY);

        [JsonIgnore]
        public Point BottomDragCenter => new Point((TopLeftX + BottomRightX) / 2, BottomRightY);

        [JsonIgnore]
        public Point LeftDragCenter => new Point(TopLeftX, (TopLeftY + BottomRightY) / 2);

        [JsonIgnore]
        public Point RightDragCenter => new Point(BottomRightX, (TopLeftY + BottomRightY) / 2);

        public Point[] AllDragCenters()
        {
            return new[]
            {
                TopDragCenter,
                BottomDragCenter,
                LeftDragCenter,
                RightDragCenter
            };
        }

        public static Rectangle GetPbDragRectangle(Point pbCenter)
        {
            return new Rectangle(
                pbCenter.X - PbDragPointSize / 2,
                pbCenter.Y - PbDragPointSize / 2,
                PbDragPointSize,
                PbDragPointSize);
        }
    }
}