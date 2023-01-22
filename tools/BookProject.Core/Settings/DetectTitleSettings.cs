namespace BookProject.Core.Settings
{
    public class DetectTitleSettings
    {
        public int MinLeft { get; set; }
        public int MinRight { get; set; }
        public int MinTop { get; set; }
        public int MinBottom { get; set; }
        public int GaussSigma1 { get; set; }
        public int GaussSigma2 { get; set; }
        public int MaxLineDist { get; set; }
        public int AddPadding { get; set; }
    }
}