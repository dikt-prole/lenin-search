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

        protected override Bitmap RenderOriginalBitmap(string imageFile)
        {
            var internalValues = new Dictionary<string, object>();
            var garbageRects = _garbageDetector.Detect(imageFile, _settings, null, internalValues);

            var originalImage = internalValues[GarbageDetector.SmoothBitmapKey] as Bitmap;
            using var g = Graphics.FromImage(originalImage);

            var width = originalImage.Width;
            var height = originalImage.Height;

            using var linePen = new Pen(Color.LimeGreen, 2);
            g.DrawLine(linePen, _settings.MinLeft, 0, _settings.MinLeft, height);
            g.DrawLine(linePen, width - _settings.MinRight, 0, width - _settings.MinRight, height);

            using var garbageRectPen = new Pen(BookProjectPalette.GetColor(OldBookProjectLabel.Garbage), 2);
            foreach (var rect in garbageRects)
            {
                g.DrawRectangle(garbageRectPen, rect);
            }

            return originalImage;
        }
    }
}