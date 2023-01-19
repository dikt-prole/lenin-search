using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using LeninSearch.Studio.Core.Detectors;
using LeninSearch.Studio.Core.Settings;

namespace LeninSearch.Studio.WinForms.ImageRendering
{
    public class TestDetectTitleImageRenderer : IImageRenderer
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

        public void RenderJpeg(string imageFile, Stream outStream)
        {
            var internalValues = new Dictionary<string, object>();
            var rectangles = _titleDetector.Detect(imageFile, _settings, null, internalValues);

            using var invertedGray = internalValues["invertedGray"] as Bitmap;
            using var g = Graphics.FromImage(invertedGray);
            using var linePen = new Pen(Color.LimeGreen, 2);

            var width = invertedGray.Width;
            var height = invertedGray.Height;

            g.DrawLine(linePen, 0, _settings.MinTop, width, _settings.MinTop);
            g.DrawLine(linePen, 0, height - _settings.MinBottom, width, height - _settings.MinBottom);
            g.DrawLine(linePen, _settings.MinLeft, 0, _settings.MinLeft, height);
            g.DrawLine(linePen, width - _settings.MinRight, 0, width - _settings.MinRight, height);


            using var rectPen = new Pen(Color.Red, 2);
            foreach (var rect in rectangles)
            {
                g.DrawRectangle(rectPen, rect);
            }

            invertedGray.Save(outStream, ImageFormat.Jpeg);
        }
    }
}