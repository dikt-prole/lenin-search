using System.Drawing;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionVertex
    {
        public string X { get; set; }
        public string Y { get; set; }

        public Point Point()
        {
            int.TryParse(X, out var x);
            int.TryParse(Y, out var y);
            return new Point(x, y);
        }
    }
}