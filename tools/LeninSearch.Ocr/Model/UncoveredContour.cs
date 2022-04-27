using System.Drawing;
using System.IO;

namespace LeninSearch.Ocr.Model
{
    public class UncoveredContour
    {
        public string ImageFile { get; set; }
        public OcrWord Word { get; set; }
        public Rectangle Rectangle { get; set; }
        public string Filename => Path.GetFileNameWithoutExtension(ImageFile);
    }
}