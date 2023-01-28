using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Detectors;
using BookProject.Core.Misc;
using BookProject.Core.Settings;

namespace BookProject.Core.ImageRendering
{
    public class TestDetectImageImageRenderer : ImageRendererBase
    {
        private readonly DetectImageSettings _settings;
        private readonly IImageDetector _imageDetector;

        public TestDetectImageImageRenderer(DetectImageSettings settings, IImageDetector imageDetector)
        {
            _settings = settings;
            _imageDetector = imageDetector;
        }

        public TestDetectImageImageRenderer(DetectImageSettings settings) : this(settings, new ImageDetector())
        { }

        public override void Render(Bitmap originalBitmap, Graphics g)
        {
            var internalValues = new Dictionary<string, object>();
            var titleRects = _imageDetector.Detect(originalBitmap, _settings, null, internalValues);

            originalBitmap = internalValues[ImageDetector.SmoothBitmapKey] as Bitmap;

            DrawOriginalBitmap(originalBitmap, g);

            var width = originalBitmap.Width;
            var height = originalBitmap.Height;
            using var linePen = new Pen(Color.LimeGreen, 2);
            DrawLine(0, _settings.MinTop, width, _settings.MinTop, linePen, g, originalBitmap);
            DrawLine(0, height - _settings.MinBottom, width, height - _settings.MinBottom, linePen, g, originalBitmap);
            DrawLine(_settings.MinLeft, 0, _settings.MinLeft, height, linePen, g, originalBitmap);
            DrawLine(width - _settings.MinRight, 0, width - _settings.MinRight, height, linePen, g, originalBitmap);

            using var titleRectPen = new Pen(BookProjectPalette.GarbageBlockColor, 2);
            foreach (var rect in titleRects)
            {
                DrawRect(rect, titleRectPen, g, originalBitmap);
            }
        }
    }
}