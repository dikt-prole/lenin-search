namespace LeninSearch.Ocr
{
    public class ImageBlock
    {
        public string FileName { get; set; }
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }
    }
}