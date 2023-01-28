using System.Drawing;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class Block
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

        [JsonProperty("stt")]
        public BlockState State { get; set; }

        [JsonIgnore]
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        [JsonIgnore]
        public Rectangle TopDragRectangle => new Rectangle(
            (TopLeftX + BottomRightX) / 2 - DragPointSize / 2,
            TopLeftY - DragPointSize / 2,
            DragPointSize,
            DragPointSize);

        [JsonIgnore]
        public Rectangle BottomDragRectangle => new Rectangle(
            (TopLeftX + BottomRightX) / 2 - DragPointSize / 2,
            BottomRightY - DragPointSize / 2,
            DragPointSize,
            DragPointSize);

        [JsonIgnore]
        public Rectangle LeftDragRectangle => new Rectangle(
            TopLeftX - DragPointSize / 2,
            (TopLeftY + BottomRightY) / 2 - DragPointSize / 2,
            DragPointSize,
            DragPointSize);

        [JsonIgnore]
        public Rectangle RightDragRectangle => new Rectangle(
            BottomRightX - DragPointSize / 2,
            (TopLeftY + BottomRightY) / 2 - DragPointSize / 2,
            DragPointSize,
            DragPointSize);

        public Rectangle[] AllDragRectangles()
        {
            return new[]
            {
                TopDragRectangle,
                BottomDragRectangle,
                LeftDragRectangle,
                RightDragRectangle
            };
        }

        public bool IsInDragRectangle(Point point)
        {
            return
                TopDragRectangle.Contains(point) ||
                BottomDragRectangle.Contains(point) ||
                LeftDragRectangle.Contains(point) ||
                RightDragRectangle.Contains(point);
        }
    }
}