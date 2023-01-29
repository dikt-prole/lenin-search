using System.Drawing;
using BookProject.Core.Misc;

namespace BookProject.Core.ImageRendering
{
    public abstract class ImageRendererBase : IImageRenderer
    {
        public abstract void Render(Bitmap originalImage, Graphics g);

        protected void DrawOriginalLine(
            Point originalPointFrom, 
            Point originalPointTo, 
            Pen pen, 
            Graphics pictureBoxGraphics, 
            Bitmap originalImage)
        {
            var pbPointFrom = pictureBoxGraphics.ToPictureBoxPoint(originalPointFrom, originalImage);
            var pbPointTo = pictureBoxGraphics.ToPictureBoxPoint(originalPointTo, originalImage);
            pictureBoxGraphics.DrawLine(pen, pbPointFrom, pbPointTo);
        }

        protected void DrawOriginalLine(
            int originalFromX,
            int originalFromY,
            int originalToX,
            int originalToY,
            Pen pen,
            Graphics pictureBoxGraphics,
            Bitmap originalImage)
        {
            DrawOriginalLine(new Point(originalFromX, originalFromY), new Point(originalToX, originalToY), pen,
                pictureBoxGraphics, originalImage);
        }

        protected void DrawOriginalRect(
            Rectangle originalRect,
            Pen pen,
            Graphics pictureBoxGraphics,
            Bitmap originalImage)
        {
            var pbRect = pictureBoxGraphics.ToPictureBoxRectangle(originalRect, originalImage);
            pictureBoxGraphics.DrawRectangle(pen, pbRect);
        }

        protected void FillOriginalRect(
            Rectangle originalRect,
            Brush brush,
            Graphics pictureBoxGraphics,
            Bitmap originalImage)
        {
            var pbRect = pictureBoxGraphics.ToPictureBoxRectangle(originalRect, originalImage);
            pictureBoxGraphics.FillRectangle(brush, pbRect);
        }

        protected void DrawOriginalBitmap(
            Bitmap originalBitmap,
            Graphics pictureBoxGraphics)
        {
            var pbHeight = pictureBoxGraphics.VisibleClipBounds.Height;
            var pbWidth = pictureBoxGraphics.VisibleClipBounds.Width;

            var scale = 1.0 * pbHeight / originalBitmap.Height;

            var scaledImageWidth = (int)(originalBitmap.Width * scale);

            var leftPadding = (pbWidth - scaledImageWidth) / 2;

            pictureBoxGraphics.DrawImage(originalBitmap, leftPadding, 0, scaledImageWidth, pbHeight);
        }
    }
}