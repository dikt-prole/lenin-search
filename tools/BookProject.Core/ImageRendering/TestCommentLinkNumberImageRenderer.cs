using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Detectors;
using BookProject.Core.Settings;

namespace BookProject.Core.ImageRendering
{
    public class TestCommentLinkNumberImageRenderer : ImageRendererBase
    {
        private readonly ICommentLinkNumberDetector _commentLinkNumberDetector;
        private readonly DetectCommentLinkNumberSettings _settings;
        public TestCommentLinkNumberImageRenderer(DetectCommentLinkNumberSettings settings) : this(new CommentLinkNumberDetector(), settings) { }

        public TestCommentLinkNumberImageRenderer(ICommentLinkNumberDetector commentLinkNumberDetector, DetectCommentLinkNumberSettings settings)
        {
            _commentLinkNumberDetector = commentLinkNumberDetector;
            _settings = settings;
        }

        protected override Bitmap RenderOriginalBitmap(string imageFile)
        {
            var internalValues = new Dictionary<string, object>();
            var linkRects = _commentLinkNumberDetector.Detect(imageFile, _settings, null, internalValues);

            var originalBitmap = internalValues[CommentLinkNumberDetector.SmoothBitmapKey] as Bitmap;
            var lineRects = internalValues[CommentLinkNumberDetector.MatchLineRectangleKey] as Rectangle[];
            using var g = Graphics.FromImage(originalBitmap);

            var linePen = new Pen(Color.GreenYellow, 2);
            foreach (var lineRect in lineRects)
            {
                g.DrawRectangle(linePen, lineRect);
            }
            
            var linkPen = new Pen(Color.DeepSkyBlue, 2);
            foreach (var linkRect in linkRects)
            {
                g.DrawRectangle(linkPen, linkRect);
            }

            return originalBitmap;
        }
    }
}