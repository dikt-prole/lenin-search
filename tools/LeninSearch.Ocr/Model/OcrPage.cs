using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeninSearch.Ocr.CV;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrPage
    {
        [JsonProperty("fn")]
        public string Filename { get; set; }

        [JsonProperty("td")]
        public DividerLine TopDivider { get; set; }

        [JsonProperty("bd")]
        public DividerLine BottomDivider { get; set; }

        [JsonProperty("lns")]
        public List<OcrLine> Lines { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("ibs")]
        public List<OcrImageBlock> ImageBlocks { get; set; }

        [JsonIgnore]
        public int ImageIndex => int.Parse(new string(Filename.Where(char.IsNumber).ToArray()));

        [JsonIgnore]
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

        public void ReindexLines()
        {
            if (Lines == null) return;

            for (var i = 0; i < Lines.Count; i++) Lines[i].LineIndex = i;
        }
    }
}