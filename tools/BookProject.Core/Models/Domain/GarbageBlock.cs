using System.Drawing;

namespace BookProject.Core.Models.Domain
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

        public static GarbageBlock Copy(GarbageBlock proto)
        {
            return new GarbageBlock
            {
                TopLeftX = proto.TopLeftX,
                TopLeftY = proto.TopLeftY,
                BottomRightX = proto.BottomRightX,
                BottomRightY = proto.BottomRightY
            };
        }
    }
}