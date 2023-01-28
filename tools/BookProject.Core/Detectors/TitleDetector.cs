using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.Detectors
{
    public class TitleDetector : ITitleDetector
    {
        public const string SmoothBitmapKey = "SMOOTH_BITMAP";

        public const string IntermediateRectanglesKey = "INTERMEDIATE_RECTANGLES";

        private readonly ICvUtility _cvUtility;

        public TitleDetector() : this(new CvUtility()) { }

        public TitleDetector(ICvUtility cvUtility)
        {
            _cvUtility = cvUtility;
        }

        public Rectangle[] Detect(string imageFile, DetectTitleSettings settings, Rectangle[] excludeAreas, Dictionary<string, object> internalValues)
        {
            using var image = ImageUtility.Load(imageFile);

            var contourRectangleResult =
                _cvUtility.GetContourRectangles(image, settings.GaussSigma1, settings.GaussSigma2);

            if (internalValues != null)
            {
                internalValues.Add(SmoothBitmapKey, new Bitmap(contourRectangleResult.SmoothBitmap));
            }

            var rectangles = contourRectangleResult.Rectangles.Where(r =>
                r.X > settings.MinLeft &&
                image.Width - r.X - r.Width > settings.MinRight &&
                r.Y > settings.MinTop &&
                image.Height - r.Y - r.Height > settings.MinBottom)
                .ToArray();

            rectangles = rectangles.OrderBy(r => r.Y).ToArray();

            if (excludeAreas?.Any() == true)
            {
                rectangles = rectangles.Where(r => excludeAreas.All(ea => !ea.IntersectsWith(r))).ToArray();
            }

            if (internalValues != null)
            {
                internalValues.Add(IntermediateRectanglesKey, rectangles);
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
            for (var i = 1; i < rectangles.Length; i++)
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