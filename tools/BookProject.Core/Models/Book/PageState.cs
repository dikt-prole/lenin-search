using System.Drawing;

namespace BookProject.Core.Models.Book
{
    public class PageState
    {
        public Page Page { get; set; }
        public Point? OriginalSelectionStartPoint { get; set; }
        public Point? PbSelectionStartPoint { get; set; }
        public Point OriginalMouseAt { get; set; }
        public Point PbMouseAt { get; set; }
        public Bitmap OriginalPageBitmap { get; set; }
    }
}