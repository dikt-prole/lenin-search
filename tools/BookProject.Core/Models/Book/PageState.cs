using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class PageState
    {
        public Page Page { get; set; }
        public Point? SelectionStartPoint { get; set; }
        public Point MouseAt { get; set; }
        public Bitmap OriginalPageBitmap { get; set; }
    }
}