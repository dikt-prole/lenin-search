using System.Drawing;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class CommentLinkBlock : Block
    {
        [JsonProperty("utx")]
        public string  CommentText { get; set; }

        [JsonIgnore]
        public override int PbDragPointSize => 6;

        [JsonIgnore]
        public override int BorderWidth => 2;

        public override DragPointLabel[] GetActiveDragLabels()
        {
            return new[]
            {
                DragPointLabel.TopLeft,
                DragPointLabel.BottomRight,
                DragPointLabel.TopRight,
                DragPointLabel.BottomLeft
            };
        }

        public static CommentLinkBlock FromRectangle(Rectangle rect)
        {
            return new CommentLinkBlock
            {
                TopLeftX = rect.X,
                TopLeftY = rect.Y,
                BottomRightX = rect.X + rect.Width,
                BottomRightY = rect.Y + rect.Height,
                State = BlockState.Normal 
            };
        }

        public static CommentLinkBlock Copy(CommentLinkBlock proto)
        {
            return new CommentLinkBlock
            {
                TopLeftX = proto.TopLeftX,
                TopLeftY = proto.TopLeftY,
                BottomRightX = proto.BottomRightX,
                BottomRightY = proto.BottomRightY,
                State = BlockState.Normal
            };
        }
    }
}