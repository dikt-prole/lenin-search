using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Model.UncoveredContourMatches
{
    public interface IUncoveredContourMatch
    {
        bool Match(UncoveredContour uncoveredContour);
        void Apply(BookProjectData bookProjectData, UncoveredContour contour);
    }
}