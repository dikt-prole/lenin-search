using BookProject.Core.Models.Domain;

namespace BookProject.Core.Models.Ocr
{
    public class OcrWord
    {
        public int TopLeftX { get; set; }

        public int TopLeftY { get; set; }

        public int BottomRightX { get; set; }

        public int BottomRightY { get; set; }

        public string Text { get; set; }


        public Word ToWord()
        {
            return new Word
            {
                BottomRightX = BottomRightX,
                BottomRightY = BottomRightY,
                TopLeftY = TopLeftY,
                TopLeftX = TopLeftX,
                Text = Text
            };
        }
    }
}