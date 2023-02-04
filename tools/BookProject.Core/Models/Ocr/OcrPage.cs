using System.Collections.Generic;
using System.Linq;

namespace BookProject.Core.Models.Ocr
{
    public class OcrPage
    {
        public List<OcrLine> Lines { get; set; }

        public string GetText()
        {
            if (Lines == null) return string.Empty;
            return string.Join(' ', Lines.Select(l => l.Text));
        }
    }
}