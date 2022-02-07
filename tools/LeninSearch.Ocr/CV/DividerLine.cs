using System.Drawing;

namespace LeninSearch.Ocr
{
    public class DividerLine
    {
        private const int DragPointSize = 20;
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

        public Rectangle DragRectangle => new Rectangle(RightX - DragPointSize / 2, Y - DragPointSize / 2, DragPointSize, DragPointSize);
    }
}