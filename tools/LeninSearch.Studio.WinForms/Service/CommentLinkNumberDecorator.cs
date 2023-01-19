using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Studio.WinForms.CV;
using LeninSearch.Studio.WinForms.Model;

namespace LeninSearch.Studio.WinForms.Service
{
    public class CommentLinkNumberDecorator : IOcrService
    {
        private readonly IOcrService _serviceBase;
        private readonly CommentLinkSettings _clSettings;
        private readonly Action<UncoveredContour> _commentLinkAction;

        public CommentLinkNumberDecorator(Action<UncoveredContour> commentLinkAction, CommentLinkSettings clSettings, IOcrService serviceBase)
        {
            _serviceBase = serviceBase;
            _clSettings = clSettings;
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
                var clCandidate = new UncoveredContour
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
            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(candidateRect));

            if (line == null) return false;

            var lineBottomDistance = line.BottomRightY - candidateRect.Y - candidateRect.Height;

            if (lineBottomDistance > _clSettings.MaxLineBottomDistance) return false;

            if (lineBottomDistance < _clSettings.MinLineBottomDistance) return false;

            if (!_clSettings.SizeMatch(candidateRect)) return false;

            return true;
        }
    }
}