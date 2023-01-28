using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book.Old
{
    public class OldBookProjectPage
    {
        [JsonProperty("fn")]
        public string Filename { get; set; }

        [JsonProperty("td")]
        public OldBookProjectDividerLine TopDivider { get; set; }

        [JsonProperty("bd")]
        public OldBookProjectDividerLine BottomDivider { get; set; }

        [JsonProperty("lns")]
        public List<OldBookProjectLine> Lines { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("ibs")]
        public List<OldBookProjectImageBlock> ImageBlocks { get; set; }

        [JsonProperty("tbs")]
        public List<OldBookProjectTitleBlock> TitleBlocks { get; set; }

        [JsonIgnore]
        public int ImageIndex => int.Parse(new string((Path.GetFileNameWithoutExtension(Filename) ?? string.Empty)
            .Where(char.IsNumber).ToArray()));

        [JsonIgnore]
        public List<OldBookProjectLine> NonImageBlockLines => Lines.Where(l =>
            ImageBlocks == null || ImageBlocks.All(ib => !ib.Rectangle.IntersectsWith(l.Rectangle))).ToList();

        public override string ToString()
        {
            return $"Filename: {Filename}, Lines: {Lines?.Count ?? 0}";
        }

        public void MergePage(OldBookProjectPage page)
        {
            TopDivider = page.TopDivider;
            BottomDivider = page.BottomDivider;
            Lines = page.Lines ?? new List<OldBookProjectLine>();
        }

        public void MergeLines(OldBookProjectLine intoLine, params OldBookProjectLine[] mergeLines)
        {
            foreach (var line in mergeLines) Lines.Remove(line);

            var intersectingLines = new[] { intoLine }.Concat(mergeLines).ToList();

            if (intersectingLines.Count < 2) return;

            intoLine.Words = intersectingLines.Where(l => l.Words != null).SelectMany(l => l.Words).OrderBy(w => w.TopLeftX).ToList();
            intoLine.FitRectangleToWords();
            ReindexLines();

            foreach (var word in intoLine.Words) word.LineBottomDistance = intoLine.BottomRightY - word.BottomRightY;
        }

        public List<OldBookProjectLine> BreakIntoWords(OldBookProjectLine line)
        {
            var lineIndex = Lines.IndexOf(line);

            Lines.Remove(line);

            var wordLines = new List<OldBookProjectLine>();
            foreach (var word in line.Words)
            {
                var wordLine = new OldBookProjectLine
                {
                    TopLeftX = word.TopLeftX,
                    TopLeftY = word.TopLeftY,
                    BottomRightX = word.BottomRightX,
                    BottomRightY = word.BottomRightY,
                    DisplayText = true,
                    Words = new List<OldBookProjectWord> { word },
                    Label = line.Label
                };
                wordLine.Features = OldBookProjectFeatures.Calculate(this, wordLine);
                wordLines.Add(wordLine);
            }

            Lines.InsertRange(lineIndex, wordLines);

            ReindexLines();

            return wordLines;
        }

        public void BreakLineByDistantWord(OldBookProjectLine line, int maxDistance)
        {
            if (line.Words == null) return;

            var words = line.Words.OrderBy(w => w.TopLeftX).ToList();

            if (words.Count < 2) return;

            for (var wordIndex = 1; wordIndex < words.Count; wordIndex++)
            {
                var distance = words[wordIndex].TopLeftX - words[wordIndex - 1].BottomRightX;
                if (distance > maxDistance)
                {
                    var leftWords = words.Take(wordIndex).ToList();
                    var lines = BreakIntoWords(line);
                    var leftLines = lines.Where(l => leftWords.Contains(l.Words[0])).ToList();
                    var rightLines = lines.Except(leftLines).ToList();
                    if (leftLines.Count > 1)
                    {
                        MergeLines(leftLines[0], leftLines.Skip(1).ToArray());
                        BreakLineByDistantWord(leftLines[0], maxDistance);
                    }
                    if (rightLines.Count > 1)
                    {
                        MergeLines(rightLines[0], rightLines.Skip(1).ToArray());
                        BreakLineByDistantWord(rightLines[0], maxDistance);
                    }
                    return;
                }
            }
        }

        public List<OldBookProjectLine> GetIntersectingLines(Rectangle rect)
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
                    line.Label = OldBookProjectLabel.Garbage;
                }
                else if (line.Features.AboveBottomDivider == 0)
                {
                    line.Label = OldBookProjectLabel.Comment;
                }
                else
                {
                    line.Label = OldBookProjectLabel.PMiddle;
                }
            }
        }

        public OldBookProjectLine AddContourLine(Rectangle rect)
        {
            var rectLine = new OldBookProjectLine
            {
                TopLeftX = rect.X,
                TopLeftY = rect.Y,
                BottomRightX = rect.X + rect.Width,
                BottomRightY = rect.Y + rect.Height,
                DisplayText = true,
                Words = new List<OldBookProjectWord>
                {
                    new OldBookProjectWord
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

        public IEnumerable<OldBookProjectLine> GetLabeledLines(params OldBookProjectLabel[] labels)
        {
            foreach (var line in Lines)
            {
                if (line.Label == null) continue;

                if (labels.Contains(line.Label.Value)) yield return line;
            }
        }

        public IEnumerable<OldBookProjectLine> GetExcludingLabels(params OldBookProjectLabel[] labels)
        {
            foreach (var line in Lines)
            {
                if (line.Label == null) continue;

                if (!labels.Contains(line.Label.Value)) yield return line;
            }
        }
    }
}