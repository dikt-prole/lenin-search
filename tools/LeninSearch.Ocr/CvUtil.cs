using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace LeninSearch.Ocr
{
    public static class CvUtil
    {
        public static DividerLine GetBottomDividerLine(string imageFile)
        {
            using var image = new Bitmap(Image.FromFile(imageFile));

            var minY = image.Height * 3 / 4;
            var maxY = image.Height - 1;
            var minX = 30;
            var maxX = 160;

            var lineRectangle = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            var lineImage = image.Clone(lineRectangle, image.PixelFormat);
            var lineBgr = lineImage.ToImage<Bgr, byte>();
            var dividerLine = GetAllDividerLines(lineBgr).OrderByDescending(l => l.Length).FirstOrDefault();
            if (dividerLine == null)
            {
                dividerLine = new DividerLine(image.Height, 0, image.Width);
            }
            else
            {
                dividerLine.Y += minY;
                dividerLine.LeftX += minX;
                dividerLine.RightX += minX;
            }

            return dividerLine;
        }

        public static DividerLine GetTopDividerLine(string imageFile)
        {
            using var image = new Bitmap(Image.FromFile(imageFile));

            var minY = 55;
            var maxY = 85;
            var minX = 0;
            var maxX = image.Width;

            var lineRectangle = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            var lineImage = image.Clone(lineRectangle, image.PixelFormat);
            var lineBgr = lineImage.ToImage<Bgr, byte>();
            var dividerLine = GetAllDividerLines(lineBgr).OrderByDescending(l => l.Length).FirstOrDefault();
            if (dividerLine == null)
            {
                dividerLine = new DividerLine(0, 0, image.Width);
            }
            else
            {
                dividerLine.Y += minY;
                dividerLine.LeftX += minX;
                dividerLine.RightX += minX;
            }

            return dividerLine;
        }

        public static IEnumerable<DividerLine> GetAllDividerLines(Image<Bgr, Byte> bgrImage)
        {
            using UMat gray = new UMat();
            using UMat cannyEdges = new UMat();

            CvInvoke.CvtColor(bgrImage, gray, ColorConversion.Bgr2Gray);

            double cannyThreshold = 180.0;
            double cannyThresholdLinking = 120.0;
            CvInvoke.Canny(gray, cannyEdges, cannyThreshold, cannyThresholdLinking);
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, //Distance resolution in pixel-related units
                Math.PI / 2, //Angle resolution measured in radians.
                2, //threshold
                5, //min Line width
                1); //gap between lines

            var horizontal = new LineSegment2D(new Point(0, 0), new Point(10, 0));

            var lineSource = lines.Where(l => Math.Abs(l.GetExteriorAngleDegree(horizontal)) < 3).ToList();
            var lineGroups = new List<List<LineSegment2D>>();
            while (lineSource.Any())
            {
                var attractorLine = lineSource[0];

                lineSource.Remove(attractorLine);
                var attractedLines = lineSource
                    .Where(l => Math.Abs(l.P1.Y - attractorLine.P1.Y) < 3)
                    .Where(l => GetMinDistance(attractorLine, l) < 10)
                    .ToList();

                foreach (var l in attractedLines) lineSource.Remove(l);
                attractedLines.Add(attractorLine);
                lineGroups.Add(attractedLines);
            }

            foreach (var lineGroup in lineGroups)
            {
                var allPoints = lineGroup.SelectMany(l => new[] { l.P1, l.P2 }).ToList();
                yield return new DividerLine
                {
                    Y = lineGroup.Last().P1.Y,
                    LeftX = allPoints.Select(p => p.X).Min(),
                    RightX = allPoints.Select(p => p.X).Max()
                };
            }
        }

        private static int GetMinDistance(LineSegment2D l1, LineSegment2D l2)
        {
            var distances = new List<int>
            {
                Math.Abs(l1.P1.X - l2.P1.X),
                Math.Abs(l1.P1.X - l2.P2.X),
                Math.Abs(l1.P2.X - l2.P2.X),
                Math.Abs(l1.P2.X - l2.P1.X)
            };

            return distances.Min();
        }
    }

    public class DividerLine
    {
        public DividerLine() {}

        public DividerLine(int y, int leftX, int rightX)
        {
            Y = y;
            LeftX = leftX;
            RightX = rightX;
        }

        public int Y { get; set; }
        public int LeftX { get; set; }
        public int RightX { get; set; }
        public int Length => RightX - LeftX;
    }
}