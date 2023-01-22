using System.Drawing;

namespace BookProject.Core.Utilities
{
    public interface ICvUtility
    {
        (Rectangle[] Rectangles, Bitmap SmoothBitmap) GetContourRectangles(Bitmap image, int gaussSigma1,
            int gaussSigma2);
    }
}