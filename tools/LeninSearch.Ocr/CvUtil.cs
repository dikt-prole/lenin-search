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
        public static (DividerLine TopLine, DividerLine BottomLine) GetTopBottomDividerLines(string imageFile)
        {
            Image<Bgr, Byte> bgrImage = new Image<Bgr, Byte>(imageFile);

            var dividerLines = GetAllDividerLines(bgrImage);

            var bottomLineCandidates = dividerLines
                .Where(l => l.Y > bgrImage.Height * 3 / 4)
                .Where(l => l.LeftX > 30 && l.RightX < 160)
                .OrderByDescending(l => l.Length)
                .ToList();

            var topLineCandidates = dividerLines
                .Where(l => l.Y > 55 && l.Y < 85)
                .OrderByDescending(l => l.Length)
                .ToList();

            var topLine = topLineCandidates.FirstOrDefault() ?? new DividerLine
            {
                Y = 0,
                LeftX = 0,
                RightX = bgrImage.Width
            };

            var bottomLine = bottomLineCandidates.FirstOrDefault() ?? new DividerLine
            {
                Y = bgrImage.Height,
                LeftX = 0,
                RightX = bgrImage.Width
            };

            return (topLine, bottomLine);
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
        public int Y { get; set; }
        public int LeftX { get; set; }
        public int RightX { get; set; }
        public int Length => RightX - LeftX;
    }
}