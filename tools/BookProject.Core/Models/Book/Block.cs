using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class Block
    {
        [JsonIgnore]
        public virtual int PbDragPointSize => 10;

        [JsonIgnore]
        public virtual int BorderWidth => 2;

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
        public Point BottomDragPoint => new Point((TopLeftX + BottomRightX) / 2, BottomRightY);

        [JsonIgnore]
        public Point LeftDragPoint => new Point(TopLeftX, (TopLeftY + BottomRightY) / 2);

        [JsonIgnore]
        public Point RightDragPoint => new Point(BottomRightX, (TopLeftY + BottomRightY) / 2);

        [JsonIgnore]
        public Point TopDragPoint => new Point((TopLeftX + BottomRightX) / 2, TopLeftY);

        [JsonIgnore]
        public Point TopLeftDragPoint => new Point(TopLeftX, TopLeftY);

        [JsonIgnore]
        public Point BottomLeftDragPoint => new Point(TopLeftX, BottomRightY);

        [JsonIgnore]
        public Point TopRightDragPoint => new Point(BottomRightX, TopLeftY);

        [JsonIgnore]
        public Point BottomRightDragPoint => new Point(BottomRightX, BottomRightY);

        [JsonIgnore]
        public Point CenterDragPoint => new Point((TopLeftX + BottomRightX) / 2, (TopLeftY + BottomRightY) / 2);

        public virtual Point[] GetActiveDragPoints()
        {
            var labels = GetActiveDragLabels();
            return labels.Select(GetDragPoint).ToArray();
        }

        public Point GetDragPoint(DragPointLabel label)
        {
            switch (label)
            {
                case DragPointLabel.Left:
                    return LeftDragPoint;
                case DragPointLabel.Right:
                    return RightDragPoint;
                case DragPointLabel.Bottom:
                    return BottomDragPoint;
                case DragPointLabel.Top:
                    return TopDragPoint;
                case DragPointLabel.TopLeft:
                    return TopLeftDragPoint;
                case DragPointLabel.BottomLeft:
                    return BottomLeftDragPoint;
                case DragPointLabel.TopRight:
                    return TopRightDragPoint;
                case DragPointLabel.BottomRight:
                    return BottomRightDragPoint;

                default:
                    return CenterDragPoint;
            }
        }

        public virtual DragPointLabel[] GetActiveDragLabels()
        {
            return new[]
            {
                DragPointLabel.Left,
                DragPointLabel.Right,
                DragPointLabel.Bottom,
                DragPointLabel.Center,
                DragPointLabel.Top,
                DragPointLabel.TopLeft,
                DragPointLabel.BottomLeft,
                DragPointLabel.TopRight,
                DragPointLabel.BottomRight
            };
        }

        public Rectangle GetPbDragRectangle(Point pbCenter)
        {
            return new Rectangle(
                pbCenter.X - PbDragPointSize / 2,
                pbCenter.Y - PbDragPointSize / 2,
                PbDragPointSize,
                PbDragPointSize);
        }
    }
}