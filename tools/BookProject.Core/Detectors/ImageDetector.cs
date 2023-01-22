using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.Detectors
{
    public class ImageDetector : IImageDetector
    {
        private readonly ICvUtility _cvUtility;

        public const string SmoothBitmapKey = "SMOOTH_BITMAP";

        public ImageDetector() : this(new CvUtility()) { }

        public ImageDetector(ICvUtility cvUtility)
        {
            _cvUtility = cvUtility;
        }

        public Rectangle[] Detect(string imageFile, DetectImageSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues)
        {
            using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));

            var contourRectangleResult =
                _cvUtility.GetContourRectangles(image, settings.GaussSigma1, settings.GaussSigma2);

            if (internalValues != null)
            {
                internalValues.Add(SmoothBitmapKey, contourRectangleResult.SmoothBitmap);
            }
            else
            {
                contourRectangleResult.SmoothBitmap?.Dispose();
            }

            var rectangles = contourRectangleResult.Rectangles
                .Where(r => r.X > settings.MinLeft &&
                            image.Width - r.X - r.Width > settings.MinRight &&
                            r.Y > settings.MinTop &&
                            image.Height - r.Y - r.Height > settings.MinBottom &&
                            r.Height > settings.MinHeight);

            if (excludeAreas?.Any() == true)
            {
                rectangles = rectangles.Where(r => excludeAreas.All(ea => !ea.IntersectsWith(r))).ToList();
            }

            var imageRectangles = new List<Rectangle>();
            foreach (var rect in rectangles)
            {
                imageRectangles.Add(new Rectangle(
                    rect.X - settings.AddPadding,
                    rect.Y - settings.AddPadding,
                    rect.Width + settings.AddPadding * 2,
                    rect.Height + settings.AddPadding * 2));
            }

            return imageRectangles.ToArray();
        }
    }
}