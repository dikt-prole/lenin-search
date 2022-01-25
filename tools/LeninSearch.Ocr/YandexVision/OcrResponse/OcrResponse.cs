using System.Collections.Generic;
using System.Drawing;

namespace LeninSearch.Ocr.YandexVision.OcrResponse
{
    public class Vertex
    {
        public string X { get; set; }
        public string Y { get; set; }

        public Point Point()
        {
            return new Point(int.Parse(X), int.Parse(Y));
        }
    }

    public class BoundingBox
    {
        public IList<Vertex> Vertices { get; set; }

        public Vertex TopLeft => Vertices[0];
        public Vertex BottomLeft => Vertices[1];
        public Vertex BottomRight => Vertices[2];
        public Vertex TopRight => Vertices[3];
    }

    public class Language
    {
        public string LanguageCode { get; set; }
        public double Confidence { get; set; }
    }

    public class Word
    {
        public BoundingBox BoundingBox { get; set; }
        public IList<Language> Languages { get; set; }
        public string Text { get; set; }
        public double Confidence { get; set; }
        public string EntityIndex { get; set; }
    }

    public class Line
    {
        public BoundingBox BoundingBox { get; set; }
        public IList<Word> Words { get; set; }
        public double Confidence { get; set; }
    }

    public class Block
    {
        public BoundingBox BoundingBox { get; set; }
        public IList<Line> Lines { get; set; }
    }

    public class Page
    {
        public IList<Block> Blocks { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

    public class TextDetection
    {
        public IList<Page> Pages { get; set; }
    }

    public class TextDetectionResult
    {
        public TextDetection TextDetection { get; set; }
    }

    public class OcrResult
    {
        public IList<TextDetectionResult> Results { get; set; }
    }

    public class OcrResponse
    {
        public IList<OcrResult> Results { get; set; }
    }
}