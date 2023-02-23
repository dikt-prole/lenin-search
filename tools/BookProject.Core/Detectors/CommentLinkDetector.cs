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

        public const string InvertedBitmapKey = "INVERTED_BITMAP";
        public const string LineGroupsKey = "LINE_GROUPS";

        public CommentLinkDetector(IOcrUtility ocrUtility) : this(new CvUtility(), ocrUtility) { }

        public CommentLinkDetector(ICvUtility cvUtility, IOcrUtility ocrUtility)
        {
            _cvUtility = cvUtility;
            _ocrUtility = ocrUtility;
        }

        public Rectangle[] Detect(Bitmap image, DetectCommentLinkSettings settings, Rectangle[] excludeAreas,
            Dictionary<string, object> internalValues)
        {
            var contourRectanglesResult = _cvUtility.GetContourRectangles(image);

            if (internalValues != null)
            {
                internalValues.Add(InvertedBitmapKey, contourRectanglesResult.InvertedBitmap);
            }

            var rectangleHeap = new List<object>();
            foreach (var rect in contourRectanglesResult.Rectangles)
            {
                if (rect.Height == 0 || excludeAreas?.Any(a => a.IntersectsWith(rect)) == true)
                {
                    continue;
                }
                rectangleHeap.Add(rect);
            }

            var lineGroups = new List<List<Rectangle>>();
            while (rectangleHeap.Any())
            {
                var seedRectangleObj = rectangleHeap[0];
                var seedPageWideRectangle = GetPageWideRectangle((Rectangle)seedRectangleObj, image.Width);
                var lineObjs = rectangleHeap.Where(lo =>
                    GetPageWideRectangle((Rectangle)lo, image.Width).IntersectsWith(seedPageWideRectangle)).ToArray();
                var lineRectangles = new List<Rectangle>();
                foreach (var lineObj in lineObjs)
                {
                    rectangleHeap.Remove(lineObj);
                    lineRectangles.Add((Rectangle)lineObj);
                }
                lineGroups.Add(lineRectangles);
            }

            if (internalValues != null)
            {
                internalValues.Add(LineGroupsKey, lineGroups);
            }

            return Array.Empty<Rectangle>();
        }

        private static Rectangle GetPageWideRectangle(Rectangle rect, int width)
        {
            return new Rectangle(0, rect.Y, width, rect.Height);
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

        private bool GeometricMatch(Rectangle lineRectangle, Rectangle linkRectangle, DetectCommentLinkSettings settings)
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
            DetectCommentLinkSettings settings)
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