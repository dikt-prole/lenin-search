using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace LeninSearch.Ocr.CV
{
    public static class CvUtil
    {
        public static DividerLine GetBottomDivider(string imageFile, List<FalseDividerArea> falseAreas)
        {
            using var image = new Bitmap(Image.FromFile(imageFile));

            var minY = image.Height * 1 / 2;
            var maxY = image.Height;

            var lineRectangle = new Rectangle(0, minY, image.Width, maxY - minY);
            var lineImage = image.Clone(lineRectangle, image.PixelFormat);
            var lineBgr = lineImage.ToImage<Bgr, byte>();
            var dividerLines = GetAllDividerLines(lineBgr).ToList();
            foreach (var dl in dividerLines)
            {
                dl.Y += minY;
            }

            dividerLines = dividerLines.Where(dl => falseAreas.All(fa => !fa.Match(dl))).OrderByDescending(l => l.Length).ToList();

            return dividerLines.FirstOrDefault() ?? new DividerLine(image.Height, 0, image.Width);
        }

        public static DividerLine GetTopDivider(string imageFile, List<FalseDividerArea> falseAreas)
        {
            using var image = new Bitmap(Image.FromFile(imageFile));

            var minY = 0;
            var maxY = image.Height / 4;

            var lineRectangle = new Rectangle(0, minY, image.Width, maxY - minY);
            var lineImage = image.Clone(lineRectangle, image.PixelFormat);
            var lineBgr = lineImage.ToImage<Bgr, byte>();
            var dividerLines = GetAllDividerLines(lineBgr).ToList();
            foreach (var dl in dividerLines)
            {
                dl.Y += minY;
            }

            dividerLines = dividerLines.Where(dl => falseAreas.All(fa => !fa.Match(dl))).OrderByDescending(l => l.Length).ToList();

            return dividerLines.FirstOrDefault() ?? new DividerLine(0, 0, image.Width);
        }

        public static IEnumerable<DividerLine> GetAllDividerLines(Image<Bgr, Byte> bgrImage)
        {
            using UMat gray = new UMat();
            using UMat cannyEdges = new UMat();

            CvInvoke.CvtColor(bgrImage, gray, ColorConversion.Bgr2Gray);

            double cannyThreshold = 180.0;
            double cannyThresholdLinking = 60.0;
            CvInvoke.Canny(gray, cannyEdges, cannyThreshold, cannyThresholdLinking);
            LineSegment2D[] lines = CvInvoke.HoughLinesP(
                cannyEdges,
                1, //Distance resolution in pixel-related units
                Math.PI / 2, //Angle resolution measured in radians.
                2, //threshold
                20, //min Line width
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
                    //.Where(l => GetMinDistance(attractorLine, l) < 10)
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
}