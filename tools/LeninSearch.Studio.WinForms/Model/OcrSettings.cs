namespace LeninSearch.Studio.WinForms.Model
{
    public static class OcrSettings
    {
        public const int WordCircleRadius = 15;
        public const int CommentLinkNumberMax = 100;

        public static readonly CommentLinkSettings DefaultCommentLinkSettings = new CommentLinkSettings
        {
            MinWidth = 3,
            MaxWidth = 10,
            MinHeight = 9,
            MaxHeight = 15,
            MinLineBottomDistance = 5,
            MaxLineBottomDistance = 15,
            MaxLineTopDistance = 3,
            MinLineTopDistance = -6
        };
    }
}