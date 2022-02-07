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

            var contourRectangles = CvUtil.GetContourRectangles(imageFile);

            var uncoveredRectangles = contourRectangles
                .Where(cr => page.Lines.All(l => !l.Rectangle.IntersectsWith(cr)))
                .Where(cr => page.Lines.Any(l => l.PageWideRectangle(page.Width).IntersectsWith(cr)))
                .ToList();

            if (!uncoveredRectangles.Any()) return result;

            foreach (var uncoveredRect in uncoveredRectangles)
            {
                var rectLine = new OcrLine
                {
                    TopLeftX = uncoveredRect.X,
                    TopLeftY = uncoveredRect.Y,
                    BottomRightX = uncoveredRect.X + uncoveredRect.Width,
                    BottomRightY = uncoveredRect.Y + uncoveredRect.Height,
                    DisplayText = true,
                    Words = new List<OcrWord>
                    {
                        new OcrWord
                        {
                            TopLeftX = uncoveredRect.X,
                            TopLeftY = uncoveredRect.Y,
                            BottomRightX = uncoveredRect.X + uncoveredRect.Width,
                            BottomRightY = uncoveredRect.Y + uncoveredRect.Height,
                            Text = "X"
                        }
                    }
                };

                var uncoveredContour = new UncoveredContour
                {
                    ImageFile = imageFile,
                    Rectangle = uncoveredRect,
                    Word = rectLine.Words[0]
                };
                _uncoveredContourAction?.Invoke(uncoveredContour);

                page.Lines.Add(rectLine);
            }

            page.Lines = page.Lines.OrderBy(p => p.TopLeftY).ThenBy(p => p.TopLeftX).ToList();

            for (var i = 0; i < page.Lines.Count; i++) page.Lines[i].LineIndex = i;

            return result;
        }
    }
}