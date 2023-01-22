using System.Collections.Generic;
using System.Linq;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Ocr;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionLine
    {
        public YandexVisionBoundingBox BoundingBox { get; set; }
        public IList<YandexVisionWord> Words { get; set; }
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
}