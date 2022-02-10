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
            var commentLineIndexes = Lines.Where(l => l.Label == OcrLabel.Comment).ToDictionary(l => l.LineIndex, l => l);

            foreach (var line in mergeLines) Lines.Remove(line);

            var intersectingLines = new[] { intoLine }.Concat(mergeLines).ToList();

            if (intersectingLines.Count < 2) return;

            intoLine.Words = intersectingLines.Where(l => l.Words != null).SelectMany(l => l.Words).OrderBy(w => w.TopLeftX).ToList();
            intoLine.FitRectangleToWords();
            ReindexLines();

            var commentedWords = Lines.SelectMany(l => l.Words).Where(w => w.CommentLineIndex.HasValue).ToList();
            foreach (var w in commentedWords) w.CommentLineIndex = commentLineIndexes[w.CommentLineIndex.Value].LineIndex;
        }

        public void BreakIntoWords(OcrLine line)
        {
            var commentLineIndexes = Lines.Where(l => l.Label == OcrLabel.Comment).ToDictionary(l => l.LineIndex, l => l);

            var lineIndex = Lines.IndexOf(line);

            Lines.Remove(line);

            var wordLines = new List<OcrLine>();
            foreach (var word in line.Words)
            {
                var wordLine = new OcrLine
                {
                    TopLeftX = word.TopLeftX,
                    TopLeftY = word.TopLeftY,
                    BottomRightX = word.BottomRightX,
                    BottomRightY = word.BottomRightY,
                    DisplayText = true,
                    Words = new List<OcrWord> { word },
                    Label = line.Label
                };
                wordLine.Features = OcrLineFeatures.Calculate(this, wordLine);
                wordLines.Add(wordLine);
            }

            Lines.InsertRange(lineIndex, wordLines);

            ReindexLines();

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

        public void CalculateLineDividerFeaturesAndLabels()
        {
            foreach (var line in Lines.Where(l => l.Features != null))
            {
                line.Features.BelowTopDivider = line.TopLeftY > TopDivider.Y ? 1 : 0;
                line.Features.AboveBottomDivider = line.TopLeftY < BottomDivider.Y ? 1 : 0;
                if (line.Features.BelowTopDivider == 0)
                {
                    line.Label = OcrLabel.Garbage;
                }
                else if (line.Features.AboveBottomDivider == 0)
                {
                    line.Label = OcrLabel.Comment;
                }
                else
                {
                    line.Label = OcrLabel.PMiddle;
                }
            }
        }

        public OcrLine AddContourLine(Rectangle rect)
        {
            var rectLine = new OcrLine
            {
                TopLeftX = rect.X,
                TopLeftY = rect.Y,
                BottomRightX = rect.X + rect.Width,
                BottomRightY = rect.Y + rect.Height,
                DisplayText = true,
                Words = new List<OcrWord>
                {
                    new OcrWord
                    {
                        TopLeftX = rect.X,
                        TopLeftY = rect.Y,
                        BottomRightX = rect.X + rect.Width,
                        BottomRightY = rect.Y + rect.Height,
                        Text = null
                    }
                }
            };

            Lines.Add(rectLine);

            Lines = Lines.OrderBy(p => p.TopLeftY).ThenBy(p => p.TopLeftX).ToList();

            return rectLine;
        }
    }
}