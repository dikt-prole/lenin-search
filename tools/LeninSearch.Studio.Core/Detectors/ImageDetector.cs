using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LeninSearch.Studio.Core.Settings;

namespace LeninSearch.Studio.Core.Detectors
{
    public class ImageDetector : IImageDetector
    {
        public Rectangle[] Detect(string imageFile, DetectImageSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues)
        {
            using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));
            using var bgrImage = image.ToImage<Bgr, byte>();

            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(0, 0, settings.GaussSigma1, settings.GaussSigma2)
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            if (internalValues != null)
            {
                internalValues.Add(nameof(invertedGray), new Bitmap(invertedGray.ToBitmap()));
            }

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(invertedGray, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bgrImage, contours, -1, new MCvScalar(255, 0, 0));

            var rectangles = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var cMinX = points.Select(p => p.X).Min();
                var cMaxX = points.Select(p => p.X).Max();
                var cMinY = points.Select(p => p.Y).Min();
                var cMaxY = points.Select(p => p.Y).Max();
                var rect = new Rectangle(cMinX, cMinY, cMaxX - cMinX, cMaxY - cMinY);
                if (rect.X > settings.MinLeft &&
                    image.Width - rect.X - rect.Width > settings.MinRight &&
                    rect.Y > settings.MinTop &&
                    image.Height - rect.Y - rect.Height > settings.MinBottom &&
                    rect.Height > settings.MinHeight)
                {
                    rectangles.Add(rect);
                }
            }

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