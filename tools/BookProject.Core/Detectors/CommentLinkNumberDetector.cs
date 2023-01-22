using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.Detectors
{
    public class CommentLinkNumberDetector : ICommentLinkNumberDetector
    {
        private readonly ICvUtility _cvUtility;

        public const string SmoothBitmapKey = "SMOOTH_BITMAP";
        public const string MatchLineRectangleKey = "MATCH_LINE_RECTANGLE";

        public CommentLinkNumberDetector() : this(new CvUtility()) { }

        public CommentLinkNumberDetector(ICvUtility cvUtility)
        {
            _cvUtility = cvUtility;
        }

        public Rectangle[] Detect(string imageFile, DetectCommentLinkNumberSettings settings, Rectangle[] excludeAreas,
            Dictionary<string, object> internalValues)
        {
            using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));

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
                    if (Match(lineRect, linkRect))
                    {
                        matchLineRectangles.Add(lineRect);
                        matchLinkRectangles.Add(linkRect);
                    }
                }
            }

            if (internalValues != null)
            {
                internalValues.Add(SmoothBitmapKey, new Bitmap(linkContourRectangleResult.SmoothBitmap));
                internalValues.Add(MatchLineRectangleKey, matchLineRectangles.ToArray());
            }

            if (matchLineRectangles.Any())
            {
                matchLineRectangles = matchLinkRectangles.OrderBy(r => r.Y).ToList();
                var maxWidth = matchLineRectangles.Max(x => x.Width);
                var maxHeight = matchLineRectangles.Min(x => x.Height);
                var padding = 10;
                var bitmapWidth = maxWidth + padding * 2;
                var bitmapHeight = (maxHeight + padding * 2) * matchLineRectangles.Count;
                using var ocrBitmap = new Bitmap(bitmapWidth, bitmapHeight);
                using var ocrBackgroundBrush = new SolidBrush(Color.White);
                using var ocrGraphics = Graphics.FromImage(ocrBitmap);
                ocrGraphics.FillRectangle(ocrBackgroundBrush, 0, 0, ocrBitmap.Width, ocrBitmap.Height);
                for (var i = 0; i < matchLineRectangles.Count; i++)
                {
                    var lineX = padding;
                    var lineY = i * (maxHeight + padding * 2) + padding;
                    using var croppedBitmap = CropImage(image, matchLineRectangles[i]);
                    ocrGraphics.DrawImage(croppedBitmap, lineX, lineY);
                }

                using var imageStream = new MemoryStream();
                ocrBitmap.Save(imageStream, ImageFormat.Jpeg);
                var imageBytes = imageStream.ToArray();

                Task.Run(() =>
                {
                    var ocrUtility = new YandexVisionOcrUtility();
                    var ocrPage = ocrUtility.GetPage(imageBytes).Result;
                    foreach (var ocrPageLine in ocrPage.Lines)
                    {
                        Debug.WriteLine(ocrPageLine.GetText());
                    }
                });
            }

            return matchLinkRectangles.ToArray();
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

        private bool Match(Rectangle lineRectangle, Rectangle linkRectangle)
        {
            return Math.Abs(lineRectangle.Y - linkRectangle.Y) <= 2 && 
                   linkRectangle.Height <= lineRectangle.Height * 3 / 4;

            //if (!lineRectangle.Contains(linkRectangle)) return false;

            //var lineBottom = new Rectangle(
            //    lineRectangle.X,
            //    lineRectangle.Y + 3 * lineRectangle.Height / 4,
            //    lineRectangle.Width,
            //    lineRectangle.Height / 4);

            //return !lineBottom.IntersectsWith(linkRectangle);
        }
    }
}