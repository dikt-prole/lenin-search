using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Detectors;
using BookProject.Core.Misc;
using BookProject.Core.Settings;

namespace BookProject.Core.ImageRendering
{
    public class TestDetectTitleImageRenderer : ImageRendererBase
    {
        private readonly DetectTitleSettings _settings;
        private readonly ITitleDetector _titleDetector;

        public TestDetectTitleImageRenderer(DetectTitleSettings settings, ITitleDetector titleDetector)
        {
            _settings = settings;
            _titleDetector = titleDetector;
        }

        public TestDetectTitleImageRenderer(DetectTitleSettings settings) : this(settings, new TitleDetector())
        { }

        public override void Render(Bitmap originalBitmap, Graphics g)
        {
            var internalValues = new Dictionary<string, object>();
            var titleRects = _titleDetector.Detect(originalBitmap, _settings, null, internalValues);

            originalBitmap = internalValues[TitleDetector.SmoothBitmapKey] as Bitmap;

            DrawOriginalBitmap(originalBitmap, g);

            var width = originalBitmap.Width;
            var height = originalBitmap.Height;
            using var linePen = new Pen(Color.LimeGreen, 2);
            DrawLine(0, _settings.MinTop, width, _settings.MinTop, linePen, g, originalBitmap);
            DrawLine(0, height - _settings.MinBottom, width, height - _settings.MinBottom, linePen, g, originalBitmap);
            DrawLine(_settings.MinLeft, 0, _settings.MinLeft, height, linePen, g, originalBitmap);
            DrawLine(width - _settings.MinRight, 0, width - _settings.MinRight, height, linePen, g, originalBitmap);

            var rectangles = internalValues[TitleDetector.IntermediateRectanglesKey] as List<Rectangle>;
            using var rectPen = new Pen(Color.Aqua, 2);
            foreach (var rect in rectangles)
            {
                DrawRect(rect, rectPen, g, originalBitmap);
            }

            using var titleRectPen = new Pen(BookProjectPalette.TitleBlockColor, 2);
            foreach (var rect in titleRects)
            {
                DrawRect(rect, titleRectPen, g, originalBitmap);
            }
        }
    }
}