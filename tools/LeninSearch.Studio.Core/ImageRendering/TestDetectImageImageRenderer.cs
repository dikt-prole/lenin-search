using System.Collections.Generic;
using System.Drawing;
using LeninSearch.Studio.Core.Detectors;
using LeninSearch.Studio.Core.Misc;
using LeninSearch.Studio.Core.Settings;

namespace LeninSearch.Studio.Core.ImageRendering
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

        protected override Bitmap RenderOriginalBitmap(string imageFile)
        {
            var internalValues = new Dictionary<string, object>();
            var titleRects = _imageDetector.Detect(imageFile, _settings, null, internalValues);

            var originalImage = internalValues["invertedGray"] as Bitmap;
            using var g = Graphics.FromImage(originalImage);

            var width = originalImage.Width;
            var height = originalImage.Height;

            using var linePen = new Pen(Color.LimeGreen, 2);
            g.DrawLine(linePen, 0, _settings.MinTop, width, _settings.MinTop);
            g.DrawLine(linePen, 0, height - _settings.MinBottom, width, height - _settings.MinBottom);
            g.DrawLine(linePen, _settings.MinLeft, 0, _settings.MinLeft, height);
            g.DrawLine(linePen, width - _settings.MinRight, 0, width - _settings.MinRight, height);

            using var titleRectPen = new Pen(OcrPalette.ImageBlockColor, 2);
            foreach (var rect in titleRects)
            {
                g.DrawRectangle(titleRectPen, rect);
            }

            return originalImage;
        }
    }
}