using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BookProject.Core.ImageRendering
{
    public abstract class ImageRendererBase : IImageRenderer
    {
        public void RenderJpeg(string imageFile, Stream outStream, int canvasWidth, int canvasHeight)
        {
            using var originalBitmap = RenderOriginalBitmap(imageFile);
            using var canvasBitmap = ToCanvasBitmap(originalBitmap, canvasWidth, canvasHeight);
            canvasBitmap.Save(outStream, ImageFormat.Jpeg);
        }
        public void RenderBmp(string imageFile, Stream outStream, int canvasWidth, int canvasHeight)
        {
            using var originalBitmap = RenderOriginalBitmap(imageFile);
            using var canvasBitmap = ToCanvasBitmap(originalBitmap, canvasWidth, canvasHeight);
            canvasBitmap.Save(outStream, ImageFormat.Bmp);
        }

        protected abstract Bitmap RenderOriginalBitmap(string imageFile);

        protected Bitmap ToCanvasBitmap(Bitmap originalBitmap, int canvasWidth, int canvasHeight)
        {
            var canvasBitmap = new Bitmap(canvasWidth, canvasHeight);

            using var canvasGraphics = Graphics.FromImage(canvasBitmap);

            using var canvasBackgroundBrush = new SolidBrush(Color.DarkGray);

            canvasGraphics.FillRectangle(canvasBackgroundBrush, 0, 0, canvasWidth, canvasHeight);

            var scale = 1.0 * canvasHeight / originalBitmap.Height;

            var scaledImageWidth = (int)(originalBitmap.Width * scale);

            var leftPadding = (canvasWidth - scaledImageWidth) / 2;

            canvasGraphics.DrawImage(originalBitmap, leftPadding, 0, scaledImageWidth, canvasHeight);

            return canvasBitmap;
        }
    }
}