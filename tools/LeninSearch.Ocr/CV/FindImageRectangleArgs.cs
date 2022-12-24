namespace LeninSearch.Ocr.CV
{
    public class FindImageRectangleArgs
    {
        public SmoothGaussianArgs GaussianArgs { get; set; }
        public int MaxLineHeight { get; set; }
        public int SideExpandMax { get; set; }
        public int ImageTitleAreaHeight { get; set; }
    }
}