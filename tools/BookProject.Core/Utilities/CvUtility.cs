using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace BookProject.Core.Utilities
{
    public class CvUtility : ICvUtility
    {
        public (Rectangle[] Rectangles, Bitmap SmoothBitmap) GetContourRectangles(Bitmap image, int gaussSigma1, int gaussSigma2)
        {
            using var bgrImage = image.ToImage<Bgr, byte>();

            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(0, 0, gaussSigma1, gaussSigma2)
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(invertedGray, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bgrImage, contours, -1, new MCvScalar(255, 0, 0));

            var contourRectangles = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var cMinX = points.Select(p => p.X).Min();
                var cMaxX = points.Select(p => p.X).Max();
                var cMinY = points.Select(p => p.Y).Min();
                var cMaxY = points.Select(p => p.Y).Max();
                var rect = new Rectangle(cMinX, cMinY, cMaxX - cMinX, cMaxY - cMinY);
                contourRectangles.Add(rect);
            }

            return (contourRectangles.ToArray(), new Bitmap(invertedGray.ToBitmap()));
        }

        public (Rectangle[] Rectangles, Bitmap InvertedBitmap) GetContourRectangles(Bitmap image)
        {
            using var bgrImage = image.ToImage<Bgr, byte>();

            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(invertedGray, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bgrImage, contours, -1, new MCvScalar(255, 0, 0));

            var contourRectangles = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var cMinX = points.Select(p => p.X).Min();
                var cMaxX = points.Select(p => p.X).Max();
                var cMinY = points.Select(p => p.Y).Min();
                var cMaxY = points.Select(p => p.Y).Max();
                var rect = new Rectangle(cMinX, cMinY, cMaxX - cMinX, cMaxY - cMinY);
                contourRectangles.Add(rect);
            }

            return (contourRectangles.ToArray(), new Bitmap(invertedGray.ToBitmap()));
        }
    }
}