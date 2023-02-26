using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BookProject.Core.Utilities
{
    public static class ImageUtility
    {
        public static Bitmap Load(string imagePath)
        {
            return new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imagePath))));
        }

        public static Bitmap Crop(Bitmap sourceBitmap, Rectangle cropRect)
        {
            var targetBitmap = new Bitmap(cropRect.Width, cropRect.Height);
            using Graphics g = Graphics.FromImage(targetBitmap);

            g.DrawImage(sourceBitmap, new Rectangle(0, 0, targetBitmap.Width, targetBitmap.Height),
                cropRect,
                GraphicsUnit.Pixel);

            return targetBitmap;
        }

        public static Bitmap WhiteOut(Bitmap sourceBitmap, Rectangle[] rectangles)
        {
            var resultBitmap = new Bitmap(sourceBitmap);
            using var g = Graphics.FromImage(resultBitmap);
            using var brush = new SolidBrush(Color.White);
            foreach (var rectangle in rectangles)
            {
                g.FillRectangle(brush, rectangle);
            }

            return resultBitmap;
        }

        public static byte[] GetJpegBytes(Bitmap image)
        {
            using var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            return stream.ToArray();
        }
    }
}