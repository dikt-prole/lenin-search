using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace LeninSearch.Ocr.Model
{
    public class OcrLine
    {
        //public string FileName { get; set; }
        public int LineIndex { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public OcrLineFeatures Features { get; set; }
        public OcrLabel? Label { get; set; }
        public List<OcrWord> Words { get; set; }
        public bool DisplayText { get; set; }

        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        public Rectangle PageWideRectangle(int pageWidth) => new Rectangle(0, TopLeftY, pageWidth, BottomRightY - TopLeftY);
    }
}