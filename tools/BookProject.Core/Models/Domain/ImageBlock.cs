using System.Drawing;

namespace BookProject.Core.Models.Domain
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
                BottomRightY = rect.Y + rect.Height
            };
        }

        public static ImageBlock Copy(ImageBlock proto)
        {
            return new ImageBlock
            {
                TopLeftX = proto.TopLeftX,
                TopLeftY = proto.TopLeftY,
                BottomRightX = proto.BottomRightX,
                BottomRightY = proto.BottomRightY
            };
        }
    }
}