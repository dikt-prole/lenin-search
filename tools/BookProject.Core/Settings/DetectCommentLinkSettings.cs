namespace BookProject.Core.Settings
{
    public class DetectCommentLinkSettings
    {
        public int LineGaussSigma1 { get; set; }
        public int LineGaussSigma2 { get; set; }
        public double TopDeltaMax { get; set; }
        public int BottomDeltaMin { get; set; }
        public int AddPadding { get; set; }
    }
}