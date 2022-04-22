using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public class IntersectingLineMergerDecorator : IOcrService
    {
        private readonly IOcrService _baseService;

        public IntersectingLineMergerDecorator(IOcrService baseService)
        {
            _baseService = baseService;
        }

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            var pageResult = await _baseService.GetOcrPageAsync(imageFile);

            if (!pageResult.Success) return pageResult;

            var page = pageResult.Page;

            for (var i = 0; i < page.Lines.Count; i++)
            {
                var attractorLine = page.Lines[i];
                var attractorPageWideRectangle = attractorLine.PageWideRectangle(page.Width);
                var attractedLines = page.Lines.Skip(i + 1).Where(l => l.Rectangle.IntersectsWith(attractorPageWideRectangle)).ToArray();
                if (attractedLines.Any())
                {
                    page.MergeLines(attractorLine, attractedLines);
                }
            }

            return pageResult;
        }
    }
}