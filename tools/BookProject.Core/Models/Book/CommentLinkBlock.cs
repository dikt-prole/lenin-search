using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class CommentLinkBlock : Block
    {
        public override int PbDragPointSize => 6;
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
    }
}