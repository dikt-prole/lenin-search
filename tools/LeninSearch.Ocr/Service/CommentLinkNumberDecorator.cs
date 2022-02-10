using System;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Ocr.CV;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public class CommentLinkNumberDecorator : IOcrService
    {
        private readonly IOcrService _serviceBase;
        private readonly Action<CommentLinkCandidate> _commentLinkAction;

        public CommentLinkNumberDecorator(Action<CommentLinkCandidate> commentLinkAction, IOcrService serviceBase)
        {
            _serviceBase = serviceBase;
            _commentLinkAction = commentLinkAction;
        }

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            var result = await _serviceBase.GetOcrPageAsync(imageFile);

            if (!result.Success) return result;

            var page = result.Page;

            var allWords = page.Lines.Where(l => l.Words != null).SelectMany(l => l.Words);
            var smoothUncovered = CvUtil.GetSmoothedContourRectangles(imageFile)
                .Where(cr => allWords.All(w => w.Rectangle.IntersectionPercent(cr) < 50))
                .Where(cr => page.Lines.Any(l => l.PageWideRectangle(page.Width).IntersectsWith(cr)))
                .Where(OcrSettings.CommentLink.Match)
                .ToList();

            foreach (var rect in smoothUncovered)
            {
                var rectLine = page.AddContourLine(rect);
                var clCandidate = new CommentLinkCandidate
                {
                    ImageFile = imageFile,
                    Rectangle = rect,
                    Word = rectLine.Words[0]
                };
                _commentLinkAction?.Invoke(clCandidate);
            }

            page.ReindexLines();

            return result;
        }
    }
}