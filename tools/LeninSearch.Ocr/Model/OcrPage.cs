using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        public int Width { get; set; }
        public int Height { get; set; }
        public List<OcrImageBlock> ImageBlocks { get; set; }
        public int ImageIndex => int.Parse(new string(Filename.Where(char.IsNumber).ToArray()));
        public List<OcrLine> NonImageBlockLines => Lines.Where(l =>
            ImageBlocks == null || ImageBlocks.All(ib => !ib.Rectangle.IntersectsWith(l.Rectangle))).ToList();

        public void MergeLines(OcrLine intoLine, params OcrLine[] mergeLines)
        {
            foreach (var line in mergeLines) Lines.Remove(line);

            var intersectingLines = new[] { intoLine }.Concat(mergeLines).ToList();

            if (intersectingLines.Count < 2) return;

            var commentLineIndexes = Lines.Where(l => l.Label == OcrLabel.Comment).ToDictionary(l => l.LineIndex, l => l);

            intoLine.Words = intersectingLines.Where(l => l.Words != null).SelectMany(l => l.Words).OrderBy(w => w.TopLeftX).ToList();
            intoLine.TopLeftX = intersectingLines.Select(b => b.TopLeftX).Min();
            intoLine.TopLeftY = intersectingLines.Select(b => b.TopLeftY).Min();
            intoLine.BottomRightX = intersectingLines.Select(b => b.BottomRightX).Max();
            intoLine.BottomRightY = intersectingLines.Select(b => b.BottomRightY).Max();

            for (var i = 0; i < Lines.Count; i++) Lines[i].LineIndex = i;

            var commentedWords = Lines.SelectMany(l => l.Words).Where(w => w.CommentLineIndex.HasValue).ToList();
            foreach (var w in commentedWords) w.CommentLineIndex = commentLineIndexes[w.CommentLineIndex.Value].LineIndex;
        }

        public List<OcrLine> GetIntersectingLines(Rectangle rect)
        {
            return Lines.Where(l => l.Rectangle.IntersectsWith(rect)).ToList();
        }
    }
}