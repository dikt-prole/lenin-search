using System;
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
    public class TitleDetector : ITitleDetector
    {
        public Rectangle[] Detect(string imageFile, DetectTitleSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues)
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

            var rectangles = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var minX = points.Select(p => p.X).Min();
                var maxX = points.Select(p => p.X).Max();
                var minY = points.Select(p => p.Y).Min();
                var maxY = points.Select(p => p.Y).Max();

                var rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                if (rect.X > settings.MinLeft && 
                    image.Width - rect.X - rect.Width > settings.MinRight &&
                    rect.Y > settings.MinTop &&
                    image.Height - rect.Y - rect.Height > settings.MinBottom)
                {
                    rectangles.Add(rect);
                }
            }

            rectangles = rectangles.OrderBy(r => r.Y).ToList();

            if (excludeAreas?.Any() == true)
            {
                rectangles = rectangles.Where(r => excludeAreas.All(ea => !ea.IntersectsWith(r))).ToList();
            }

            if (internalValues != null)
            {
                internalValues.Add(nameof(rectangles), rectangles);
            }

            if (!rectangles.Any())
            {
                return Array.Empty<Rectangle>();
            }

            // generate title sets
            var titleSets = new List<List<Rectangle>>
            {
                new List<Rectangle>
                {
                    rectangles[0]
                }
            };
            for (var i = 1; i < rectangles.Count; i++)
            {
                var currRect = rectangles[i];
                var prevRect = titleSets.Last().Last();
                if (currRect.Y - prevRect.Y - prevRect.Height > settings.MaxLineDist)
                {
                    titleSets.Add(new List<Rectangle>{currRect});
                }
                else
                {
                    titleSets.Last().Add(currRect);
                }
            }

            // generate title set rectangles
            var titleRectangles = new List<Rectangle>();
            foreach (var titleSet in titleSets)
            {
                var minX = titleSet.Min(r => r.X);
                var maxX = titleSet.Max(r => r.X + r.Width);
                var minY = titleSet.Min(r => r.Y);
                var maxY = titleSet.Max(r => r.Y + r.Height);
                titleRectangles.Add(
                    new Rectangle(
                        minX - settings.AddPadding, 
                        minY - settings.AddPadding, 
                        maxX - minX + settings.AddPadding * 2,
                        maxY - minY + settings.AddPadding * 2));
            }

            return titleRectangles.ToArray();
        }
    }
}