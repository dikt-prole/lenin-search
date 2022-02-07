using System.Drawing;

namespace LeninSearch.Ocr.Model
{
    public class UncoveredContour
    {
        public string ImageFile { get; set; }
        public OcrWord Word { get; set; }
        public Rectangle Rectangle { get; set; }
    }
}