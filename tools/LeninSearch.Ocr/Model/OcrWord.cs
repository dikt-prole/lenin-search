using System;

namespace LeninSearch.Ocr.Model
{
    public class OcrWord
    {
        public Guid Id { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public string Text { get; set; }
        public bool Italic { get; set; }
        public bool Bold { get; set; }
    }
}