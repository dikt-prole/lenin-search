using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using LeninSearch.Ocr.CV;

namespace LeninSearch.Script.Scripts
{
    public class AutoRotateScript : IScript
    {
        public string Id => "auto-rotate-canvas";
        public string Arguments => "images-folder";
        public void Execute(params string[] input)
        {
            var imageFolder = input[0];

            var imageFiles = Directory.GetFiles(imageFolder, "*.jpg");

            using var backgroundBrush = new SolidBrush(Color.White);
            foreach (var imageFile in imageFiles)
            {
                using var image = Image.FromFile(imageFile);

                if (image.Width <= image.Height) continue;

                var contours = CvUtil.GetContourRectangles(imageFile, SmoothGaussianArgs.SmallSmooth());
                var minX = contours.Select(c => c.X).Min();
                var maxX = contours.Select(c => c.X + c.Width).Max();
                var minY = contours.Select(c => c.Y).Min();
                var maxY = contours.Select(c => c.Y + c.Height).Max();
                var cropRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                var cropBitmap = new Bitmap(cropRect.Width, cropRect.Height);
                using (var g = Graphics.FromImage(image))
                {
                    g.DrawImage(image, 0, 0, cropRect, GraphicsUnit.Pixel);
                }
                

                var newWidth = image.Height;
                var newHeight = cropBitmap.Height * newWidth / cropBitmap.Width;
                cropBitmap = new Bitmap(cropBitmap, newWidth, newHeight);

                var resultImage = new Bitmap(image.Height, image.Width);
                using (var g = Graphics.FromImage(resultImage))
                {
                    g.FillRectangle(backgroundBrush, 0, 0, resultImage.Width, resultImage.Height);

                    g.DrawImage(cropBitmap, 0, 0);
                }

                resultImage.Save(imageFile, ImageFormat.Jpeg);
            }
        }
    }
}