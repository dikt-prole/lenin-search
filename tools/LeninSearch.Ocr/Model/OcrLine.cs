using System;
using System.Collections.Generic;

namespace LeninSearch.Ocr.Model
{
    public class OcrLine
    {
        public Guid Id { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public List<OcrWord> Words { get; set; }
    }
}