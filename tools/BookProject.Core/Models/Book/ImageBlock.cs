using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class ImageBlock : Block
    {
        public static ImageBlock FromRectangle(Rectangle rect)
        {
            return new ImageBlock
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