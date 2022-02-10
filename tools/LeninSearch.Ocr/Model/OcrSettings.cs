using System.Drawing;

namespace LeninSearch.Ocr.Model
{
    public class OcrSettings
    {
        public const int WordCircleRadius = 15;
        public const int CommentLinkNumberMax = 100;

        public static class CommentLink
        {
            public const int MinWidth = 2;
            public const int MaxWidth = 9;
            public const int MinHeight = 9;
            public const int MaxHeight = 14;

            public const int MinLineBottomDistance = 6;
            public const int MaxLineBottomDistance = 12;

            public static bool Match(Rectangle rect)
            {
                return (MinWidth <= rect.Width && rect.Width <= MaxWidth) &&
                       (MinHeight <= rect.Height && rect.Height <= MaxHeight);
            }

        }
    }
}