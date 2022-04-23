using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Script.Scripts.Models
{
    public class Fb2Line
    {
        public int TopLeftY { get; set; }
        public List<OcrLine> Lines { get; set; }
        public Fb2LineType Type { get; set; }
        public string GetText()
        {
            var sb = new StringBuilder();
            var lastLineEndedWithDash = false;
            foreach (var line in Lines)
            {
                var lastWord = line.Words.Last();
                var currentLineEndedWithDash = Type == Fb2LineType.Paragraph && lastWord.Text.EndsWith('-');

                if (currentLineEndedWithDash) lastWord.Text = lastWord.Text.TrimEnd('-');

                if (!lastLineEndedWithDash) sb.Append(" ");

                sb.Append(string.Join(" ", line.Words.Select(w => w.Text)));

                lastLineEndedWithDash = currentLineEndedWithDash;
            }

            return sb.ToString();
        }
        public string GetXml()
        {
            switch (Type)
            {
                case Fb2LineType.Paragraph:
                    return $"<p>{GetText()}</p>";
                case Fb2LineType.Title:
                    return $"<title><p>{GetText()}</p></title>";
            }

            return null;
        }

        public static Fb2Line Construct(OcrLine line)
        {
            var fb2Line = new Fb2Line
            {
                Type = line.Label == OcrLabel.Title 
                    ? Fb2LineType.Title 
                    : Fb2LineType.Paragraph,
                Lines = new List<OcrLine> { line },
                TopLeftY = line.TopLeftY
            };

            return fb2Line;
        }
    }

    public enum Fb2LineType
    {
        Paragraph, Image, Title
    }
}