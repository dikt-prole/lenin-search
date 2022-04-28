using System.Linq;

namespace LeninSearch.Ocr.Model.UncoveredContourMatches
{
    public class LineStartMatch : IUncoveredContourMatch
    {
        public bool Match(UncoveredContour uncoveredContour)
        {
            var page = uncoveredContour.Page;

            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(uncoveredContour.Rectangle));

            if (line != null && line.Rectangle.IntersectsWith(uncoveredContour.Rectangle)) return false;

            if (uncoveredContour.Word.TopLeftX > 50) return false;

            if (uncoveredContour.Rectangle.Width > 15) return false;

            if (uncoveredContour.Rectangle.Height > 20) return false;

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