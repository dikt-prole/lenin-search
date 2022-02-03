using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LeninSearch.Ocr.Model
{
    public class OcrLine
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int LineIndex { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public OcrLineFeatures Features { get; set; }
        public OcrLabel? Label { get; set; }
        public List<OcrWord> Words { get; set; }
        public int ImageIndex => int.Parse(new string(FileName).Where(char.IsNumber).ToArray());
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        public override string ToString()
        {
            if (Label == null) return $"{FileName}-{LineIndex}";

            return $"{FileName}-{LineIndex} ({Label})";
        }
    }
}