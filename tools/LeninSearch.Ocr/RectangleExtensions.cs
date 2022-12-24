using System.Drawing;

namespace LeninSearch.Ocr
{
    public static class RectangleExtensions
    {
        public static int GetIntersectionWidth(this Rectangle r, Rectangle other)
        {
            if (!r.IntersectsWith(other)) return 0;

            var intersection = new Rectangle(r.Location, r.Size);
            intersection.Intersect(other);
            return intersection.Width;
        }
    }
}