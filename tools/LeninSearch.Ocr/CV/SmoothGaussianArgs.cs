namespace LeninSearch.Ocr.CV
{
    public class SmoothGaussianArgs
    {
        public int KernelWidth { get; set; }
        public int KernelHeight { get; set; }
        public int Sigma1 { get; set; }
        public int Sigma2 { get; set; }

        public static SmoothGaussianArgs SmallSmooth()
        {
            return new SmoothGaussianArgs
            {
                KernelHeight = 0,
                KernelWidth = 0,
                Sigma1 = 1,
                Sigma2 = 1
            };
        }

        public static SmoothGaussianArgs MediumSmooth()
        {
            return new SmoothGaussianArgs
            {
                KernelHeight = 0,
                KernelWidth = 0,
                Sigma1 = 2,
                Sigma2 = 2
            };
        }

        public static SmoothGaussianArgs LargeSmooth()
        {
            return new SmoothGaussianArgs
            {
                KernelHeight = 0,
                KernelWidth = 0,
                Sigma1 = 2,
                Sigma2 = 2
            };
        }
    }
}