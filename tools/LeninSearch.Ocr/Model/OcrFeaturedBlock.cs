using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using LeninSearch.Ocr.Labeling;

namespace LeninSearch.Ocr.Model
{
    public class OcrFeaturedBlock
    {
        public string FileName { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
        public List<OcrLine> Lines { get; set; }
        public OcrBlockLabel? Label { get; set; }
        public OcrBlockFeatures Features { get; set; }
        public int BlockIndex { get; set; }
        public int ImageIndex => int.Parse(new string(FileName).Where(char.IsNumber).ToArray());
        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);

        public override string ToString()
        {
            if (Label == null) return $"{FileName}-{BlockIndex}";

            return $"{FileName}-{BlockIndex} ({Label})";
        }
    }
}