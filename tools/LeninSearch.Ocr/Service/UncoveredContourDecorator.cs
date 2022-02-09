using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Ocr.CV;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public class UncoveredContourDecorator : IOcrService
    {
        private readonly IOcrService _serviceBase;
        private readonly Action<UncoveredContour> _uncoveredContourAction;

        public UncoveredContourDecorator(Action<UncoveredContour> uncoveredContourAction, IOcrService serviceBase)
        {
            _serviceBase = serviceBase;
            _uncoveredContourAction = uncoveredContourAction;
        }

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            var result = await _serviceBase.GetOcrPageAsync(imageFile);

            if (!result.Success) return result;

            var page = result.Page;

            var allWords = page.Lines.Where(l => l.Words != null).SelectMany(l => l.Words);
            var smoothUncovered = CvUtil.GetSmoothedContourRectangles(imageFile)
                .Where(cr => allWords.All(w => !w.Rectangle.IntersectsWith(cr)))
                .Where(cr => page.Lines.Any(l => l.PageWideRectangle(page.Width).IntersectsWith(cr)))
                .Where(cr => OcrSettings.UncoveredContourMinHeight <= cr.Height && cr.Height <= OcrSettings.UncoveredContourMaxHeight)
                .Where(cr => cr.Width <= OcrSettings.UncoveredContourMaxWidth)
                .ToList();

            foreach (var rect in smoothUncovered)
            {
                var rectLine = page.AddContourLine(rect);
                var uncoveredContour = new UncoveredContour
                {
                    ImageFile = imageFile,
                    Rectangle = rect,
                    Word = rectLine.Words[0]
                };
                _uncoveredContourAction?.Invoke(uncoveredContour);
            }

            allWords = page.Lines.Where(l => l.Words != null).SelectMany(l => l.Words);
            var uncovered = CvUtil.GetContourRectangles(imageFile)
                .Where(cr => allWords.All(w => !w.Rectangle.IntersectsWith(cr)))
                .Where(cr => page.Lines.Any(l => l.PageWideRectangle(page.Width).IntersectsWith(cr)))
                .Where(cr => OcrSettings.UncoveredContourMinHeight <= cr.Height  && cr.Height <= OcrSettings.UncoveredContourMaxHeight)
                .Where(cr => cr.Width <= OcrSettings.UncoveredContourMaxWidth)
                .ToList();

            foreach (var rect in uncovered)
            {
                var rectLine = page.AddContourLine(rect);
                var uncoveredContour = new UncoveredContour
                {
                    ImageFile = imageFile,
                    Rectangle = rect,
                    Word = rectLine.Words[0]
                };
                _uncoveredContourAction?.Invoke(uncoveredContour);
            }

            page.ReindexLines();

            return result;
        }
    }
}