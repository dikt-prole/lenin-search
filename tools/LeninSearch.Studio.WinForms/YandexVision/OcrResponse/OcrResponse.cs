using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeninSearch.Studio.Core.Models;
using LeninSearch.Studio.WinForms.Model;

namespace LeninSearch.Studio.WinForms.YandexVision.OcrResponse
{
    public class Vertex
    {
        public string X { get; set; }
        public string Y { get; set; }

        public Point Point()
        {
            int.TryParse(X, out var x);
            int.TryParse(Y, out var y);
            return new Point(x, y);
        }
    }

    public class BoundingBox
    {
        public IList<Vertex> Vertices { get; set; }

        public Vertex TopLeft => Vertices[0];
        public Vertex BottomLeft => Vertices[1];
        public Vertex BottomRight => Vertices[2];
        public Vertex TopRight => Vertices[3];

        public bool IsSameY(BoundingBox box)
        {
            var topLeft = TopLeft.Point();
            var bottomLeft = BottomLeft.Point();

            var boxTopLeft = box.TopLeft.Point();
            var boxBottomLeft = box.BottomLeft.Point();

            if (topLeft.Y <= boxTopLeft.Y && boxTopLeft.Y <= bottomLeft.Y) return true;

            if (topLeft.Y <= boxBottomLeft.Y && boxBottomLeft.Y <= bottomLeft.Y) return true;

            if (boxTopLeft.Y <= topLeft.Y && topLeft.Y <= boxBottomLeft.Y) return true;

            if (boxTopLeft.Y <= bottomLeft.Y && bottomLeft.Y <= boxBottomLeft.Y) return true;

            return false;
        }

        public bool Contains(int x, int y)
        {
            var topLeft = TopLeft.Point();
            var bottomLeft = BottomLeft.Point();
            var topRight = TopRight.Point();
            var bottomRight = BottomRight.Point();

            if (x > topRight.X) return false;

            if (x < topLeft.X) return false;

            if (y > bottomLeft.Y) return false;

            if (y < topLeft.Y) return false;

            return true;
        }

        public Rectangle Rectangle()
        {
            var topLeft = TopLeft.Point();

            var bottomRight = BottomRight.Point();

            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
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

        public OcrWord ToOcrWord()
        {
            var topLeft = BoundingBox.TopLeft.Point();
            var bottomRight = BoundingBox.BottomRight.Point();
            return new OcrWord
            {
                TopLeftX = topLeft.X,
                TopLeftY = topLeft.Y,
                BottomRightX = bottomRight.X,
                BottomRightY = bottomRight.Y,
                Text = Text
            };
        }
    }

    public class YandexVisionLine
    {
        public BoundingBox BoundingBox { get; set; }
        public IList<Word> Words { get; set; }
        public double Confidence { get; set; }

        public OcrLine ToOcrLine()
        {
            var topLeft = BoundingBox.TopLeft.Point();
            var bottomRight = BoundingBox.BottomRight.Point();
            return new OcrLine
            {
                TopLeftX = topLeft.X,
                TopLeftY = topLeft.Y,
                BottomRightX = bottomRight.X,
                BottomRightY = bottomRight.Y,
                Words = Words.Select(w => w.ToOcrWord()).ToList()
            };
        }
    }

    public class OcrBlock
    {
        public BoundingBox BoundingBox { get; set; }
        public IList<YandexVisionLine> Lines { get; set; }
    }

    public class Page
    {
        public IList<OcrBlock> Blocks { get; set; }
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