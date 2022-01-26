using System;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace LeninSearch.Ocr
{
    public static class CvUtil
    {
        public static (int? TopY, int? BottomY) GetPageLines(string imageFile)
        {
            var cannyThreshold = 180.0;
            var cannyThresholdLinking = 120.0;

            using UMat gray = new UMat();
            using UMat cannyEdges = new UMat();

            Image<Bgr, Byte> img1 = new Image<Bgr, Byte>(imageFile);
            CvInvoke.CvtColor(img1, gray, ColorConversion.Bgr2Gray);
            CvInvoke.Canny(gray, cannyEdges, cannyThreshold, cannyThresholdLinking);
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, //Distance resolution in pixel-related units
                Math.PI / 2, //Angle resolution measured in radians.
                2, //threshold
                50, //min Line width
                1); //gap between lines

            var testLine = new LineSegment2D(new Point(0, 0), new Point(10, 0));

            lines = lines.Where(l => Math.Abs(l.GetExteriorAngleDegree(testLine)) < 5).ToArray();

            var topLines = lines.Where(l => l.P1.Y < img1.Height / 2).OrderBy(l => l.P1.Y).ToArray();
            var bottomLines = lines.Where(l => l.P1.Y > img1.Height / 2).OrderByDescending(l => l.P1.Y).ToArray();

            if (!topLines.Any() && !bottomLines.Any()) return (null, null);

            if (!topLines.Any()) return (null, bottomLines[0].P1.Y);

            if (!bottomLines.Any()) return (topLines[0].P1.Y, null);

            return (topLines[0].P1.Y, bottomLines[0].P1.Y);
        }
    }
}