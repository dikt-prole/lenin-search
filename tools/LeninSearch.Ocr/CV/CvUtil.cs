using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

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

            var bottomDividerY = dividerLines.FirstOrDefault()?.Y ?? image.Height - 40;
            return new DividerLine(bottomDividerY, 20, image.Width - 20);
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

            var topDividerY = dividerLines.FirstOrDefault()?.Y ?? 40;
            return new DividerLine(topDividerY, 20, image.Width - 20);
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

        public static IEnumerable<Rectangle> GetSmoothedContourRectangles(string imageFile)
        {
            using var image = new Bitmap(Image.FromFile(imageFile));
            using var bgrImage = image.ToImage<Bgr, byte>();
            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(0, 0, 3, 3)
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(invertedGray, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bgrImage, contours, -1, new MCvScalar(255, 0, 0));

            var rectangleSource = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var minX = points.Select(p => p.X).Min();
                var maxX = points.Select(p => p.X).Max();
                var minY = points.Select(p => p.Y).Min();
                var maxY = points.Select(p => p.Y).Max();

                var rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                rectangleSource.Add(rect);
            }

            while (rectangleSource.Any())
            {
                var attractor = rectangleSource[0];
                rectangleSource.Remove(attractor);
                var attracted = rectangleSource.Where(r => r.IntersectsWith(attractor)).ToList();
                foreach (var r in attracted) rectangleSource.Remove(r);
                attracted.Add(attractor);

                var minX = attracted.Select(r => r.X).Min();
                var maxX = attracted.Select(r => r.X + r.Width).Max();
                var minY = attracted.Select(r => r.Y).Min();
                var maxY = attracted.Select(r => r.Y + r.Height).Max();

                var rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                if (rect.Width > 0 && rect.Height > 0) yield return rect;
            }
        }

        public static IEnumerable<Rectangle> GetContourRectangles(string imageFile)
        {
            using var image = new Bitmap(Image.FromFile(imageFile));
            using var bgrImage = image.ToImage<Bgr, byte>();
            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(0, 0, 3, 3)
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(invertedGray, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bgrImage, contours, -1, new MCvScalar(255, 0, 0));

            var rectangleSource = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var minX = points.Select(p => p.X).Min();
                var maxX = points.Select(p => p.X).Max();
                var minY = points.Select(p => p.Y).Min();
                var maxY = points.Select(p => p.Y).Max();

                var rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);
                rectangleSource.Add(rect);
            }

            while (rectangleSource.Any())
            {
                var attractor = rectangleSource[0];
                rectangleSource.Remove(attractor);
                var attracted = rectangleSource.Where(r => r.IntersectsWith(attractor)).ToList();
                foreach (var r in attracted) rectangleSource.Remove(r);
                attracted.Add(attractor);

                var minX = attracted.Select(r => r.X).Min();
                var maxX = attracted.Select(r => r.X + r.Width).Max();
                var minY = attracted.Select(r => r.Y).Min();
                var maxY = attracted.Select(r => r.Y + r.Height).Max();

                var rect = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                if (rect.Width > 0 && rect.Height > 0) yield return rect;
            }
        }
    }
}