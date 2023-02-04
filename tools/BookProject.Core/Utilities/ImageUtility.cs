using System.Drawing;
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
    }
}