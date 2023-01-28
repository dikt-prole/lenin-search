using System.Drawing;

namespace BookProject.Core.Misc
{
    public static class GraphicsExtensions
    {
        public static Rectangle ToPictureBoxRectangle(this Graphics pbGraphics, Rectangle originalRectangle, Bitmap originalImage)
        {
            var pbHeight = pbGraphics.VisibleClipBounds.Height;
            var pbWidth = pbGraphics.VisibleClipBounds.Width;
            var originalToPb = 1.0 * pbHeight / originalImage.Height;
            var pbRectY = (int)(originalRectangle.Y * originalToPb);
            var pbLeftMargin = (pbWidth - originalImage.Width * originalToPb) / 2;
            var pbRectX = (int)(originalRectangle.X * originalToPb + pbLeftMargin);
            var pbRectWidth = (int)(originalRectangle.Size.Width * originalToPb);
            var pbRectHeight = (int)(originalRectangle.Size.Height * originalToPb);
            return new Rectangle(pbRectX, pbRectY, pbRectWidth, pbRectHeight);
        }

        public static Point ToPictureBoxPoint(this Graphics pbGraphics, Point originalPoint, Bitmap originalImage)
        {
            var pbHeight = pbGraphics.VisibleClipBounds.Height;
            var pbWidth = pbGraphics.VisibleClipBounds.Width;
            var originalToPb = 1.0 * pbHeight / originalImage.Height;
            var pbY = (int)(originalPoint.Y * originalToPb);
            var pbLeftMargin = (pbWidth - originalImage.Width * originalToPb) / 2;
            var pbX = (int)(originalPoint.X * originalToPb + pbLeftMargin);
            return new Point(pbX, pbY);
        }
    }
}