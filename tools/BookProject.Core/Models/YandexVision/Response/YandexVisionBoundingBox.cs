using System.Collections.Generic;
using System.Drawing;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionBoundingBox
    {
        public IList<YandexVisionVertex> Vertices { get; set; }

        public YandexVisionVertex TopLeft => Vertices[0];
        public YandexVisionVertex BottomLeft => Vertices[1];
        public YandexVisionVertex BottomRight => Vertices[2];
        public YandexVisionVertex TopRight => Vertices[3];

        public bool IsSameY(YandexVisionBoundingBox box)
        {
            var topLeft = TopLeft.Point();
            var bottomLeft = BottomLeft.Point();

            var boxTopLeft = box.TopLeft.Point();
            var boxBottomLeft = box.BottomLeft.Point();

            if (topLeft.Y <= boxTopLeft.Y && boxTopLeft.Y <= bottomLeft.Y) return true;

            if (topLeft.Y <= boxBottomLeft.Y && boxBottomLeft.Y <= bottomLeft.Y) return true;

            if (boxTopLeft.Y <= topLeft.Y && topLeft.Y <= boxBottomLeft.Y) return true;

            if (boxTopLeft.Y <= bottomLeft.Y && bottomLeft.Y <= boxBottomLeft.Y) return true;

            return false;
        }

        public bool Contains(int x, int y)
        {
            var topLeft = TopLeft.Point();
            var bottomLeft = BottomLeft.Point();
            var topRight = TopRight.Point();
            var bottomRight = BottomRight.Point();

            if (x > topRight.X) return false;

            if (x < topLeft.X) return false;

            if (y > bottomLeft.Y) return false;

            if (y < topLeft.Y) return false;

            return true;
        }

        public Rectangle Rectangle()
        {
            var topLeft = TopLeft.Point();

            var bottomRight = BottomRight.Point();

            return new Rectangle(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        }
    }
}