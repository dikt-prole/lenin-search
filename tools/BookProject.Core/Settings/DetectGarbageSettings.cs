namespace BookProject.Core.Settings
{
    public class DetectGarbageSettings
    {
        public int MinLeft { get; set; }
        public int MinRight { get; set; }
        public int MinHeight { get; set; }
        public int MaxHeight { get; set; }
        public int GaussSigma1 { get; set; }
        public int GaussSigma2 { get; set; }
        public int AddPadding { get; set; }
    }
}