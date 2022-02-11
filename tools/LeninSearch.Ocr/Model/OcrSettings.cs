using System.Drawing;

namespace LeninSearch.Ocr.Model
{
    public static class OcrSettings
    {
        public const int WordCircleRadius = 15;
        public const int CommentLinkNumberMax = 100;

        public static readonly CommentLinkSettings DefaultCommentLinkSettings = new CommentLinkSettings
        {
            MinWidth = 1,
            MaxWidth = 10,
            MinHeight = 8,
            MaxHeight = 15,
            MinLineBottomDistance = 5,
            MaxLineBottomDistance = 15
        };
    }
}