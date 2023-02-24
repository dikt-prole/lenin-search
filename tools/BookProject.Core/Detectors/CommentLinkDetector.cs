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

            var lineRectanglesResult =
                _cvUtility.GetContourRectangles(image, settings.LineGaussSigma1, settings.LineGaussSigma2);

            var lineGroups = new List<List<Rectangle>>();
            foreach (var lineRectangle in lineRectanglesResult.Rectangles)
            {
                var lineGroup = contourRectanglesResult.Rectangles.Where(r => r.IntersectsWith(lineRectangle)).ToList();
                if (lineGroup.Any())
                {
                    lineGroups.Add(lineGroup);
                }
            }

            if (internalValues != null)
            {
                internalValues.Add(LineGroupsKey, lineGroups);
            }

            var commentLinks = new List<Rectangle>();
            var pad = settings.AddPadding;
            foreach (var lineGroup in lineGroups)
            {
                var topLineY = lineGroup.Sum(r => r.Y) / lineGroup.Count;
                var bottomLineY = lineGroup.Sum(r => r.Y + r.Height) / lineGroup.Count;
                foreach (var r in lineGroup)
                {
                    var topDelta = Math.Abs(0.5 - 1.0 * (topLineY - r.Y) / r.Height);
                    var bottomDelta = bottomLineY - r.Y - r.Height;
                    if (topDelta <= settings.TopDeltaMax && bottomDelta > settings.BottomDeltaMin)
                    {
                        commentLinks.Add(new Rectangle(r.X - pad, r.Y - pad, r.Width + 2 * pad, r.Height + 2 * pad));
                    }
                }
            }

            return commentLinks.ToArray();
        }
    }
}