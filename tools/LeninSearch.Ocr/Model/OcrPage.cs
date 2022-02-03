using System.Collections.Generic;

namespace LeninSearch.Ocr.Model
{
    public class OcrPage
    {
        public string Filename { get; set; }
        public DividerLine TopDivider { get; set; }
        public DividerLine BottomDivider { get; set; }
        public List<OcrLine> Lines { get; set; }
    }
}