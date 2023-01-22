using System.Collections.Generic;
using System.Linq;

namespace BookProject.Core.Models.Ocr
{
    public class OcrLine
    {
        public int TopLeftX { get; set; }

        public int TopLeftY { get; set; }

        public int BottomRightX { get; set; }

        public int BottomRightY { get; set; }

        public List<OcrWord> Words { get; set; }

        public string GetText()
        {
            return Words == null ? string.Empty : string.Join(" ", Words.Select(w => w.Text));
        }
    }
}