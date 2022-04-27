namespace LeninSearch.Ocr.Model.UncoveredContourMatches
{
    public class AllMatch : IUncoveredContourMatch
    {
        public bool Match(UncoveredContour uncoveredContour)
        {
            return true;
        }

        public void Apply(OcrData ocrData, UncoveredContour contour)
        {
            return;
        }
    }
}