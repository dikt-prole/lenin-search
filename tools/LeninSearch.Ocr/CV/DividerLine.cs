namespace LeninSearch.Ocr
{
    public class DividerLine
    {
        public DividerLine() {}

        public DividerLine(int y, int leftX, int rightX)
        {
            Y = y;
            LeftX = leftX;
            RightX = rightX;
        }

        public int Y { get; set; }
        public int LeftX { get; set; }
        public int RightX { get; set; }
        public int Length => RightX - LeftX;
    }
}