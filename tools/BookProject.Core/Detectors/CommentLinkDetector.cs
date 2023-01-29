using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookProject.Core.Misc;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.Detectors
{
    public class CommentLinkDetector : ICommentLinkNumberDetector
    {
        private readonly ICvUtility _cvUtility;
        private readonly IOcrUtility _ocrUtility;

        public const string SmoothBitmapKey = "SMOOTH_BITMAP";
        public const string MatchLineRectangleKey = "MATCH_LINE_RECTANGLE";

        public CommentLinkDetector(IOcrUtility ocrUtility) : this(new CvUtility(), ocrUtility) { }

        public CommentLinkDetector(ICvUtility cvUtility, IOcrUtility ocrUtility)
        {
            _cvUtility = cvUtility;
            _ocrUtility = ocrUtility;
        }

        public Rectangle[] Detect(Bitmap image, DetectCommentLinkNumberSettings settings, Rectangle[] excludeAreas,
            Dictionary<string, object> internalValues)
        {

            var lineContourRectangleResult =
                _cvUtility.GetContourRectangles(image, settings.LineGaussSigma1, settings.LineGaussSigma2);

            var linkContourRectangleResult =
                _cvUtility.GetContourRectangles(image, settings.LinkGaussSigma1, settings.LinkGaussSigma2);

            var matchLinkRectangles = new List<Rectangle>();
            var matchLineRectangles = new List<Rectangle>();
            foreach (var lineRect in lineContourRectangleResult.Rectangles)
            {
                foreach (var linkRect in linkContourRectangleResult.Rectangles)
                {
                    if (GeometricMatch(lineRect, linkRect, settings))
                    {
                        if (excludeAreas == null || excludeAreas.All(ea => !ea.IntersectsWith(linkRect)))
                        {
                            matchLineRectangles.Add(lineRect);
                            matchLinkRectangles.Add(linkRect);
                        }
                    }
                }
            }

            matchLinkRectangles = FilterByAllowedSymbols(image, matchLinkRectangles.ToArray(), settings);
            matchLineRectangles = matchLineRectangles.Where(l => matchLinkRectangles.Any(l.IntersectsWith)).ToList();

            if (internalValues != null)
            {
                internalValues.Add(SmoothBitmapKey, new Bitmap(linkContourRectangleResult.SmoothBitmap));
                internalValues.Add(MatchLineRectangleKey, matchLineRectangles.ToArray());
            }

            return matchLinkRectangles.Select(r => r.AddPadding(settings.AddPadding)).ToArray();
        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private bool GeometricMatch(Rectangle lineRectangle, Rectangle linkRectangle, DetectCommentLinkNumberSettings settings)
        {
            return 
                linkRectangle.Height > 0 && 
                linkRectangle.Width > 0 &&
                Math.Abs(lineRectangle.Y - linkRectangle.Y) <= settings.LineTopDistanceMax && 
                linkRectangle.Height <= lineRectangle.Height * settings.LineHeightPartMax;
        }

        private List<Rectangle> FilterByAllowedSymbols(
            Bitmap originalBitmap, 
            Rectangle[] candidateRectangles,
            DetectCommentLinkNumberSettings settings)
        {
            if (candidateRectangles.Length == 0)
            {
                return candidateRectangles.ToList();
            }

            candidateRectangles = candidateRectangles.OrderBy(r => r.Y).ToArray();

            var ocrCellWidth = candidateRectangles.Max(x => x.Width);
            var ocrCellHeight = candidateRectangles.Min(x => x.Height);
            var ocrCellPadding = 10;
            var ocrBitmapWidth = ocrCellWidth + ocrCellPadding * 2;
            var ocrBitmapHeight = (ocrCellHeight + ocrCellPadding * 2) * candidateRectangles.Length;

            using var ocrBitmap = new Bitmap(ocrBitmapWidth, ocrBitmapHeight);
            using var ocrBackgroundBrush = new SolidBrush(Color.White);
            using var ocrGraphics = Graphics.FromImage(ocrBitmap);

            ocrGraphics.FillRectangle(ocrBackgroundBrush, 0, 0, ocrBitmap.Width, ocrBitmap.Height);
            for (var i = 0; i < candidateRectangles.Length; i++)
            {
                var ocrCellX = ocrCellPadding;
                var ocrCellY = i * (ocrCellHeight + ocrCellPadding * 2) + ocrCellPadding;
                using var croppedBitmap = CropImage(originalBitmap, candidateRectangles[i]);
                ocrGraphics.DrawImage(croppedBitmap, ocrCellX, ocrCellY);
            }

            using var imageStream = new MemoryStream();
            ocrBitmap.Save(imageStream, ImageFormat.Jpeg);
            var imageBytes = imageStream.ToArray();
            var ocrPage = Task.Run(() => _ocrUtility.GetPageAsync(imageBytes)).Result;

            var resultRectangles = new List<Rectangle>();
            for (var i = 0; i < candidateRectangles.Length; i++)
            {
                var ocrCellX = ocrCellPadding;
                var ocrCellY = i * (ocrCellHeight + ocrCellPadding * 2) + ocrCellPadding;
                var ocrCellRectangle = new Rectangle(
                    ocrCellX,
                    ocrCellY,
                    ocrCellWidth + ocrCellPadding * 2,
                    ocrCellHeight + ocrCellPadding * 2);
                var ocrLine = ocrPage.Lines.FirstOrDefault(l => l.Rectangle.IntersectsWith(ocrCellRectangle));
                
                if (ocrLine == null || string.IsNullOrEmpty(ocrLine.Text)) continue;

                if (settings.AllowedSymbols.Any(s => ocrLine.Text.Contains(s)))
                {
                    resultRectangles.Add(candidateRectangles[i]);
                }
            }

            return resultRectangles;
        }
    }
}