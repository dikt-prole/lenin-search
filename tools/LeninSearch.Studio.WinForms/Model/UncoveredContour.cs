using System.Drawing;
using System.IO;
using LeninSearch.Studio.Core.Models;

namespace LeninSearch.Studio.WinForms.Model
{
    public class UncoveredContour
    {
        public string ImageFile { get; set; }
        public OcrWord Word { get; set; }
        public OcrPage Page { get; set; }
        public Rectangle Rectangle { get; set; }
        public string Filename => Path.GetFileNameWithoutExtension(ImageFile);
    }
}