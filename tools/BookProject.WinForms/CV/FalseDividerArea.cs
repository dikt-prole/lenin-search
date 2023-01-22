using BookProject.Core.Models.Book;

namespace BookProject.WinForms.CV
{
    public class FalseDividerArea
    {
        public FalseDividerArea(int minY, int maxY)
        {
            MinY = minY;
            MaxY = maxY;
        }

        public int MinY { get; set; }
        public int MaxY { get; set; }

        public bool Match(BookProjectDividerLine divider)
        {
            return MinY <= divider.Y && divider.Y <= MaxY;
        }
    }
}