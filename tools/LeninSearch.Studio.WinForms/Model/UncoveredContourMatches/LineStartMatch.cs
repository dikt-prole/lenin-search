using System.Linq;

namespace LeninSearch.Studio.WinForms.Model.UncoveredContourMatches
{
    public class LineStartMatch : IUncoveredContourMatch
    {
        public bool Match(UncoveredContour uncoveredContour)
        {
            var page = uncoveredContour.Page;

            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(uncoveredContour.Rectangle));

            if (line != null && line.Rectangle.IntersectsWith(uncoveredContour.Rectangle)) return false;

            if (uncoveredContour.Word.TopLeftX > 100) return false;

            if (uncoveredContour.Rectangle.Width > 25) return false;

            if (uncoveredContour.Rectangle.Height > 30) return false;

            return true;
        }

        public void Apply(OcrData ocrData, UncoveredContour contour)
        {
            var page = ocrData.Pages.FirstOrDefault(p => p.Filename == contour.Filename);

            if (page == null) return;

            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(contour.Rectangle));

            if (line?.Words == null) return;

            line.Words.Add(contour.Word);

            line.Words = line.Words.OrderBy(w => w.TopLeftX).ToList();

            line.FitRectangleToWords();
        }
    }
}