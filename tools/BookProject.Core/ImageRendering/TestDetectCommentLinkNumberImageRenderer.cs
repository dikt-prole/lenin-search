using System.Collections.Generic;
using System.Drawing;
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
            var linkRects = _commentLinkNumberDetector.Detect(originalBitmap, _settings, null, internalValues);

            originalBitmap = internalValues[CommentLinkDetector.SmoothBitmapKey] as Bitmap;
            var lineRects = internalValues[CommentLinkDetector.MatchLineRectangleKey] as Rectangle[];

            DrawOriginalBitmap(originalBitmap, g);
            var linePen = new Pen(Color.GreenYellow, 2);
            foreach (var lineRect in lineRects)
            {
                DrawOriginalRect(lineRect, linePen, g, originalBitmap);
            }

            var linkPen = new Pen(Color.DeepSkyBlue, 2);
            foreach (var linkRect in linkRects)
            {
                DrawOriginalRect(linkRect, linkPen, g, originalBitmap);
            }
        }
    }
}