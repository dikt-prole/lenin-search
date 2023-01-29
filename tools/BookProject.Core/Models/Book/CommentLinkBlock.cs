using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class CommentLinkBlock : Block
    {
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