using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class GarbageBlock : Block
    {
        public static GarbageBlock FromRectangle(Rectangle rect)
        {
            return new GarbageBlock
            {
                TopLeftX = rect.X,
                TopLeftY = rect.Y,
                BottomRightX = rect.X + rect.Width,
                BottomRightY = rect.Y + rect.Height
            };
        }
    }
}