using System.Collections.Generic;
using System.Linq;
using Accord.Math.Geometry;

namespace LeninSearch.Ocr.Model
{
    public class OcrPage
    {
        public string Filename { get; set; }
        public DividerLine TopDivider { get; set; }
        public DividerLine BottomDivider { get; set; }
        public List<OcrLine> Lines { get; set; }

        public void MergeLines(OcrLine intoLine, params OcrLine[] mergeLines)
        {
            foreach (var line in mergeLines) Lines.Remove(line);

            var intersectingLines = new[] { intoLine }.Concat(mergeLines).ToList();
            intoLine.Words = intersectingLines.Where(l => l.Words != null).SelectMany(l => l.Words).ToList();
            intoLine.TopLeftX = intersectingLines.Select(b => b.TopLeftX).Min();
            intoLine.TopLeftY = intersectingLines.Select(b => b.TopLeftY).Min();
            intoLine.BottomRightX = intersectingLines.Select(b => b.BottomRightX).Max();
            intoLine.BottomRightY = intersectingLines.Select(b => b.BottomRightY).Max();

            for (var i = 0; i < Lines.Count; i++) Lines[i].LineIndex = i;
        }
    }
}