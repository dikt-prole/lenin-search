using System.Drawing;

namespace LeninSearch.Ocr.Model
{
    public class OcrImageBlock
    {
        private const int DragPointSize = 20;
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }
        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }

        public Rectangle Rectangle => new Rectangle(TopLeftX, TopLeftY, BottomRightX - TopLeftX, BottomRightY - TopLeftY);
        public Rectangle TopLeftRectangle => new Rectangle(TopLeftX - DragPointSize / 2, TopLeftY - DragPointSize / 2, DragPointSize, DragPointSize);
        public Rectangle BottomRightRectangle => new Rectangle(BottomRightX - DragPointSize / 2, BottomRightY - DragPointSize / 2, DragPointSize, DragPointSize);
    }
}