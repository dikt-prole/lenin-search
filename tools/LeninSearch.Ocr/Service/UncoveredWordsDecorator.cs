using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Ocr.CV;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public class UncoveredWordsDecorator : IOcrService
    {
        private readonly IOcrService _serviceBase;

        public UncoveredWordsDecorator(IOcrService serviceBase)
        {
            _serviceBase = serviceBase;
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

            var lines = uncoveredRectangles.Select(r => new OcrLine
            {
                TopLeftX = r.X,
                TopLeftY = r.Y,
                BottomRightX = r.X + r.Width,
                BottomRightY = r.Y + r.Height,
                DisplayText = true,
                Words = new List<OcrWord>
                {
                    new OcrWord
                    {
                        TopLeftX = r.X,
                        TopLeftY = r.Y,
                        BottomRightX = r.X + r.Width,
                        BottomRightY = r.Y + r.Height,
                        Text = "X"
                    }
                }
            }).ToList();

            page.Lines.AddRange(lines);

            page.Lines = page.Lines.OrderBy(p => p.TopLeftY).ThenBy(p => p.TopLeftX).ToList();

            for (var i = 0; i < page.Lines.Count; i++) page.Lines[i].LineIndex = i;

            return result;
        }
    }
}