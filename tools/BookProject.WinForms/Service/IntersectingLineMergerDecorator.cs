using System.Linq;
using System.Threading.Tasks;
using BookProject.Core.Models.Book;
using BookProject.WinForms.YandexVision;

namespace BookProject.WinForms.Service
{
    public class IntersectingLineMergerDecorator : IPageProvider
    {
        private readonly IPageProvider _baseService;

        public IntersectingLineMergerDecorator(IPageProvider baseService)
        {
            _baseService = baseService;
        }

        public async Task<(BookProjectPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
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