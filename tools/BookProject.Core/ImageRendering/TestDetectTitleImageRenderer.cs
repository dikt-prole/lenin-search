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

        protected override Bitmap RenderOriginalBitmap(string imageFile)
        {
            var internalValues = new Dictionary<string, object>();
            var titleRects = _titleDetector.Detect(imageFile, _settings, null, internalValues);

            var originalImage = internalValues[TitleDetector.SmoothBitmapKey] as Bitmap;
            using var g = Graphics.FromImage(originalImage);

            var width = originalImage.Width;
            var height = originalImage.Height;

            using var linePen = new Pen(Color.LimeGreen, 2);
            g.DrawLine(linePen, 0, _settings.MinTop, width, _settings.MinTop);
            g.DrawLine(linePen, 0, height - _settings.MinBottom, width, height - _settings.MinBottom);
            g.DrawLine(linePen, _settings.MinLeft, 0, _settings.MinLeft, height);
            g.DrawLine(linePen, width - _settings.MinRight, 0, width - _settings.MinRight, height);

            var rectangles = internalValues[TitleDetector.IntermediateRectanglesKey] as List<Rectangle>;
            using var rectPen = new Pen(Color.Aqua, 2);
            foreach (var rect in rectangles)
            {
                g.DrawRectangle(rectPen, rect);
            }

            using var titleRectPen = new Pen(OcrPalette.TitleBlockColor, 2);
            foreach (var rect in titleRects)
            {
                g.DrawRectangle(titleRectPen, rect);
            }

            return originalImage;
        }
    }
}