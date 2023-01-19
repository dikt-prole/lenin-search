using System.Linq;

namespace LeninSearch.Studio.WinForms.Model.UncoveredContourMatches
{
    public class CommentLinkNumberMatch : IUncoveredContourMatch
    {
        public bool Match(UncoveredContour contour)
        {
            if (contour.Word.LineBottomDistance > 15) return false;
            if (contour.Word.LineBottomDistance < 5) return false;
            if (contour.Word.LineTopDistance > 3) return false;
            if (contour.Word.LineTopDistance < -6) return false;

            return (3 <= contour.Rectangle.Width && contour.Rectangle.Width <= 10) &&
                   (9 <= contour.Rectangle.Height && contour.Rectangle.Height <= 15);
        }

        public void Apply(OcrData ocrData, UncoveredContour contour)
        {
            var page = ocrData.Pages.FirstOrDefault(p => p.Filename == contour.Filename);

            if (page == null) return;

            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(contour.Rectangle));

            if (line?.Words == null) return;

            contour.Word.IsCommentLinkNumber = true;

            line.Words.Add(contour.Word);

            line.Words = line.Words.OrderBy(w => w.TopLeftX).ToList();

            line.FitRectangleToWords();
        }
    }
}