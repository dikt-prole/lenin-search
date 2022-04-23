using System.Collections.Generic;
using System.Linq;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Script.Scripts.Models
{
    public class Fb2Line
    {
        public int TopLeftY { get; set; }
        public List<OcrWord> Words { get; set; }
        public Fb2LineType Type { get; set; }
        public string GetText()
        {
            return string.Join(" ", Words.Select(w => w.Text));
        }
        public string GetXml()
        {
            switch (Type)
            {
                case Fb2LineType.Paragraph:
                    return $"<p>{GetText()}</p>";
                case Fb2LineType.Title:
                    return $"<title><p>{GetText()}</p><title/>";
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
                Words = line.Words.ToArray().ToList(),
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