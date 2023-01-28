using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;

namespace BookProject.WinForms.Model.UncoveredContourMatches
{
    public interface IUncoveredContourMatch
    {
        bool Match(UncoveredContour uncoveredContour);
        void Apply(OldBookProjectData bookProjectData, UncoveredContour contour);
    }
}