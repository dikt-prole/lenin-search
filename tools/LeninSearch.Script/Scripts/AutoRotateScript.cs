using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.Diagnostics;
using LeninSearch.Ocr.CV;
using Debug = System.Diagnostics.Debug;

namespace LeninSearch.Script.Scripts
{
    public class AutoRotateScript : IScript
    {
        public string Id => "auto-rotate-canvas";
        public string Arguments => "images-folder";

        private const int Padding = 20;
        public void Execute(params string[] input)
        {
            var imageFolder = input[0];

            var imageFiles = Directory.GetFiles(imageFolder, "*.jpg");

            using var backgroundBrush = new SolidBrush(Color.White);
            foreach (var imageFile in imageFiles)
            {
                Console.WriteLine(imageFile);
                using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));

                if (image.Width <= image.Height) continue;

                SaveToTmp(imageFile, image);

                var contours = CvUtil.GetContourRectangles(imageFile, SmoothGaussianArgs.SmallSmooth());
                var minX = contours.Select(c => c.X).Min();
                var maxX = contours.Select(c => c.X + c.Width).Max();
                var minY = contours.Select(c => c.Y).Min();
                var maxY = contours.Select(c => c.Y + c.Height).Max();
                var cropRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                var cropBitmap = new Bitmap(cropRect.Width, cropRect.Height);
                using (var g = Graphics.FromImage(cropBitmap))
                {
                    g.DrawImage(image, 0, 0, cropRect, GraphicsUnit.Pixel);
                }

                var newWidth = image.Height - Padding * 2;
                var newHeight = cropBitmap.Height * newWidth / cropBitmap.Width;
                cropBitmap = new Bitmap(cropBitmap, newWidth, newHeight);

                var resultImage = new Bitmap(image.Height, image.Width);
                using (var g = Graphics.FromImage(resultImage))
                {
                    g.FillRectangle(backgroundBrush, 0, 0, resultImage.Width, resultImage.Height);

                    g.DrawImage(cropBitmap, Padding, Padding);
                }
                
                File.Delete(imageFile);
                resultImage.Save(imageFile, ImageFormat.Jpeg);
            }
        }

        private void SaveToTmp(string imageFile, Bitmap b)
        {
            var tmpFile = Path.GetTempFileName() + ".jpg";
            b.Save(tmpFile, ImageFormat.Jpeg);
            Debug.WriteLine($"{Path.GetFileName(imageFile)} => {tmpFile}");
        }
    }
}