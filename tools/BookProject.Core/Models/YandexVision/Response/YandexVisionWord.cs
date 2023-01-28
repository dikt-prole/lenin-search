using System.Collections.Generic;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;
using BookProject.Core.Models.Ocr;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionWord
    {
        public YandexVisionBoundingBox BoundingBox { get; set; }
        public IList<YandexVisionLanguage> Languages { get; set; }
        public string Text { get; set; }
        public double Confidence { get; set; }
        public string EntityIndex { get; set; }

        public OldBookProjectWord ToBookProjectWord()
        {
            var topLeft = BoundingBox.TopLeft.Point();
            var bottomRight = BoundingBox.BottomRight.Point();
            return new OldBookProjectWord
            {
                TopLeftX = topLeft.X,
                TopLeftY = topLeft.Y,
                BottomRightX = bottomRight.X,
                BottomRightY = bottomRight.Y,
                Text = Text
            };
        }

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
}