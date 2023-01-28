using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.Detectors
{
    public class GarbageDetector : IGarbageDetector
    {
        private readonly ICvUtility _cvUtility;

        public const string SmoothBitmapKey = "SMOOTH_BITMAP";

        public GarbageDetector() : this(new CvUtility())
        { }

        public GarbageDetector(ICvUtility cvUtility)
        {
            _cvUtility = cvUtility;
        }

        public Rectangle[] Detect(Bitmap image, DetectGarbageSettings settings, Rectangle[] excludeAreas,
            Dictionary<string, object> internalValues)
        {
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

            var matchRectangles = contourRectangleResult.Rectangles
                .Where(r => (r.X > settings.MinLeft && image.Width - r.X - r.Width > settings.MinRight) ||
                            r.Height < settings.MinHeight ||
                            r.Height > settings.MaxHeight);

            var garbageRectangles = matchRectangles.Select(r => new Rectangle(
                r.X - settings.AddPadding,
                r.Y - settings.AddPadding,
                r.Width + settings.AddPadding * 2,
                r.Height + settings.AddPadding * 2)).ToArray();

            return garbageRectangles;
        }
    }
}