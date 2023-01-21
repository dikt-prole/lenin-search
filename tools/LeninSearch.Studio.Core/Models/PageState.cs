using System.Drawing;

namespace LeninSearch.Studio.Core.Models
{
    public class PageState
    {
        public OcrPage Page { get; set; }
        public Point? SelectionStartPoint { get; set; }
    }
}