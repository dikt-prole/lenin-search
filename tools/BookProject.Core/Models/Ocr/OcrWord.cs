namespace BookProject.Core.Models.Ocr
{
    public class OcrWord
    {
        public int TopLeftX { get; set; }

        public int TopLeftY { get; set; }

        public int BottomRightX { get; set; }

        public int BottomRightY { get; set; }

        public string Text { get; set; }
    }
}