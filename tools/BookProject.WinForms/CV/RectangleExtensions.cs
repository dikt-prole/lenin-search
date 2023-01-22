using System.Drawing;

namespace BookProject.WinForms.CV
{
    public static class RectangleExtensions
    {
        public static double IntersectionPercent(this Rectangle rect, Rectangle intersectingRect)
        {
            var rectSquare = 1.0 * rect.Width * rect.Height;

            intersectingRect.Intersect(rect);

            var intersectionSquare = 1.0 * intersectingRect.Width * intersectingRect.Height;

            return 100 * intersectionSquare / rectSquare;
        }
    }
}