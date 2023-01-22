using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class PageState
    {
        public BookProjectPage Page { get; set; }
        public Point? SelectionStartPoint { get; set; }
    }
}