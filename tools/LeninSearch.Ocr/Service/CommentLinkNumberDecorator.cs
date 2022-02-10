using System;
using System.Drawing;
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

            var smoothUncovered = CvUtil.GetSmoothedContourRectangles(imageFile)
                .Where(cr => Match(cr, page))
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

        private bool Match(Rectangle candidateRect, OcrPage page)
        {
            var allWords = page.Lines.Where(l => l.Words != null).SelectMany(l => l.Words);

            if (allWords.Any(w => w.Rectangle.IntersectionPercent(candidateRect) > 50)) return false;

            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(candidateRect));

            if (line == null) return false;

            var lineBottomDistance = line.BottomRightY - candidateRect.Y - candidateRect.Height;

            if (lineBottomDistance > OcrSettings.CommentLink.MaxLineBottomDistance) return false;

            if (lineBottomDistance < OcrSettings.CommentLink.MinLineBottomDistance) return false;

            if (!OcrSettings.CommentLink.Match(candidateRect)) return false;

            return true;
        }
    }
}