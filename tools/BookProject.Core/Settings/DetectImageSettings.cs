using System.Drawing;

namespace BookProject.Core.Settings
{
    public class DetectImageSettings
    {
        public int MinLeft { get; set; }
        public int MinRight { get; set; }
        public int MinTop { get; set; }
        public int MinBottom { get; set; }
        public int GaussSigma1 { get; set; }
        public int GaussSigma2 { get; set; }
        public int MinHeight { get; set; }
        public int AddPadding { get; set; }

        public bool Match(Rectangle rect, int imageWidth, int imageHeight)
        {
            return rect.X > MinLeft &&
                   imageWidth - rect.X - rect.Width > MinRight &&
                   rect.Y > MinTop &&
                   imageHeight - rect.Y - rect.Height > MinBottom &&
                   rect.Height > MinHeight;
        }
    }
}