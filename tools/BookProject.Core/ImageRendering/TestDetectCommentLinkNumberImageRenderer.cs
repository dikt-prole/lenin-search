using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BookProject.Core.Detectors;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.ImageRendering
{
    public class TestDetectCommentLinkNumberImageRenderer : ImageRendererBase
    {
        private readonly ICommentLinkNumberDetector _commentLinkNumberDetector;
        private readonly DetectCommentLinkSettings _settings;
        public TestDetectCommentLinkNumberImageRenderer(DetectCommentLinkSettings settings, IOcrUtility ocrUtility) : this(new CommentLinkDetector(ocrUtility), settings) { }

        public TestDetectCommentLinkNumberImageRenderer(ICommentLinkNumberDetector commentLinkNumberDetector, DetectCommentLinkSettings settings)
        {
            _commentLinkNumberDetector = commentLinkNumberDetector;
            _settings = settings;
        }

        public override void Render(Bitmap originalBitmap, Graphics g)
        {
            var internalValues = new Dictionary<string, object>();
            var commentLinks = _commentLinkNumberDetector.Detect(originalBitmap, _settings, null, internalValues);

            originalBitmap = internalValues[CommentLinkDetector.InvertedBitmapKey] as Bitmap;
            var lineGroups = internalValues[CommentLinkDetector.LineGroupsKey] as List<List<Rectangle>>;

            DrawOriginalBitmap(originalBitmap, g);
            using var lineBrush = new SolidBrush(Color.FromArgb(50, Color.Aqua));
            using var linePen = new Pen(Color.Red);
            foreach (var lineGroup in lineGroups)
            {
                var minX = lineGroup.Min(r => r.X);
                var maxX = lineGroup.Max(r => r.X + r.Width);
                var minY = lineGroup.Min(r => r.Y);
                var maxY = lineGroup.Max(r => r.Y + r.Height);
                var lineRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                FillOriginalRect(lineRect, lineBrush, g, originalBitmap);

                var topLineY = lineGroup.Sum(r => r.Y) / lineGroup.Count;
                var bottomLineY = lineGroup.Sum(r => r.Y + r.Height) / lineGroup.Count;
                DrawOriginalLine(lineRect.X, topLineY, lineRect.X + lineRect.Width, topLineY, linePen, g, originalBitmap);
                DrawOriginalLine(lineRect.X, bottomLineY, lineRect.X + lineRect.Width, bottomLineY, linePen, g, originalBitmap);
            }

            foreach (var commentLink in commentLinks)
            {
                DrawOriginalRect(commentLink, Pens.Blue, g, originalBitmap);
            }
        }
    }
}