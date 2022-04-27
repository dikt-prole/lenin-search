namespace LeninSearch.Ocr.Model.UncoveredContourMatches
{
    public interface IUncoveredContourMatch
    {
        bool Match(UncoveredContour uncoveredContour);
        void Apply(OcrData ocrData, UncoveredContour contour);
    }
}