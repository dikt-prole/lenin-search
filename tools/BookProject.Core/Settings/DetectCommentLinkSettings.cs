namespace BookProject.Core.Settings
{
    public class DetectCommentLinkSettings
    {
        public int MinWidth { get; set; }
        public int MinHeight { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public int LinkGaussSigma1 { get; set; }
        public int LinkGaussSigma2 { get; set; }
        public int LineGaussSigma1 { get; set; }
        public int LineGaussSigma2 { get; set; }
        public double LineHeightPartMax { get; set; }
        public int LineTopDistanceMax { get; set; }
        public int AddPadding { get; set; }
        public string AllowedSymbols { get; set; }
    }
}