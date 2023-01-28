using System.Collections.Generic;
using System.Drawing;
using BookProject.Core.Detectors;
using BookProject.Core.Misc;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;
using BookProject.Core.Settings;

namespace BookProject.Core.ImageRendering
{
    public class TestDetectGarbageImageRenderer : ImageRendererBase
    {
        private readonly DetectGarbageSettings _settings;
        private readonly IGarbageDetector _garbageDetector;

        public TestDetectGarbageImageRenderer(DetectGarbageSettings settings, IGarbageDetector garbageDetector)
        {
            _settings = settings;
            _garbageDetector = garbageDetector;
        }

        public TestDetectGarbageImageRenderer(DetectGarbageSettings settings) : this(settings, new GarbageDetector())
        { }

        public override void Render(Bitmap originalBitmap, Graphics g)
        {
            var internalValues = new Dictionary<string, object>();
            var garbageRects = _garbageDetector.Detect(originalBitmap, _settings, null, internalValues);

            originalBitmap = internalValues[GarbageDetector.SmoothBitmapKey] as Bitmap;

            DrawOriginalBitmap(originalBitmap, g);

            var width = originalBitmap.Width;
            var height = originalBitmap.Height;
            using var linePen = new Pen(Color.LimeGreen, 2);
            DrawLine(_settings.MinLeft, 0, _settings.MinLeft, height, linePen, g, originalBitmap);
            DrawLine(width - _settings.MinRight, 0, width - _settings.MinRight, height, linePen, g, originalBitmap);

            using var garbageRectPen = new Pen(BookProjectPalette.GetColor(OldBookProjectLabel.Garbage), 2);
            foreach (var rect in garbageRects)
            {
                DrawRect(rect, garbageRectPen, g, originalBitmap);
            }
        }
    }
}