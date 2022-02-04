using System;
using System.Drawing;

namespace LeninSearch.Ocr.Model
{
    public class OcrWord
    {
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public string Text { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
        public int? CommentLineIndex { get; set; }
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);
    }
}