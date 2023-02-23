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
            _commentLinkNumberDetector.Detect(originalBitmap, _settings, null, internalValues);

            originalBitmap = internalValues[CommentLinkDetector.InvertedBitmapKey] as Bitmap;
            var lineGroups = internalValues[CommentLinkDetector.LineGroupsKey] as List<List<Rectangle>>;

            DrawOriginalBitmap(originalBitmap, g);
            using var lineBrush = new SolidBrush(Color.FromArgb(50, Color.Aqua));
            foreach (var lineGroup in lineGroups)
            {
                var minX = lineGroup.Min(r => r.X);
                var maxX = lineGroup.Max(r => r.X + r.Width);
                var minY = lineGroup.Min(r => r.Y);
                var maxY = lineGroup.Max(r => r.Y + r.Height);
                var lineRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                FillOriginalRect(lineRect, lineBrush, g, originalBitmap);
            }
        }
    }
}