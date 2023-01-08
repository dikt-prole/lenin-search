using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CsvHelper;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.Model.UncoveredContourMatches;

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
                .SmoothGaussian(0, 0, 1, 1)
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


        public static Bitmap GetSmoothedBitmap(string imageFile, SmoothGaussianArgs smoothGaussianArgs)
        {
            using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));
            using var bgrImage = image.ToImage<Bgr, byte>();

            //using var invertedGray = bgrImage.Convert<Gray, byte>()
            //    .ThresholdBinary(new Gray(90), new Gray(255))
            //    .Not();

            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(
                    smoothGaussianArgs.KernelWidth,
                    smoothGaussianArgs.KernelHeight,
                    smoothGaussianArgs.Sigma1,
                    smoothGaussianArgs.Sigma2)
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            return invertedGray.ToBitmap();
        }

        // todo: get small contours after smoothed contours
        public static IEnumerable<Rectangle> GetContourRectangles(string imageFile, SmoothGaussianArgs smoothGaussianArgs)
        {
            using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));
            using var bgrImage = image.ToImage<Bgr, byte>();

            //using var invertedGray = bgrImage.Convert<Gray, byte>()
            //    .ThresholdBinary(new Gray(90), new Gray(255))
            //    .Not();

            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(
                    smoothGaussianArgs.KernelWidth, 
                    smoothGaussianArgs.KernelHeight, 
                    smoothGaussianArgs.Sigma1, 
                    smoothGaussianArgs.Sigma2)
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

        public static Rectangle? FindImageRectangle(string imageFile, FindImageRectangleArgs args, out Dictionary<string, object> processingData)
        {
            processingData = new Dictionary<string, object>();

            using var image = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));
            using var bgrImage = image.ToImage<Bgr, byte>();

            using var invertedGray = bgrImage.Convert<Gray, byte>()
                .SmoothGaussian(
                    args.GaussianArgs.KernelWidth,
                    args.GaussianArgs.KernelHeight,
                    args.GaussianArgs.Sigma1,
                    args.GaussianArgs.Sigma2)
                .Not()
                .ThresholdBinary(new Gray(25), new Gray(255));

            processingData.Add("SMOOTH_BITMAP", invertedGray.ToBitmap());

            var contours = new VectorOfVectorOfPoint();
            var hierarchy = new Mat();
            CvInvoke.FindContours(invertedGray, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            CvInvoke.DrawContours(bgrImage, contours, -1, new MCvScalar(255, 0, 0));

            var rects = new List<Rectangle>();
            for (var i = 0; i < contours.Size; i++)
            {
                var points = contours[i].ToArray();
                var cMinX = points.Select(p => p.X).Min();
                var cMaxX = points.Select(p => p.X).Max();
                var cMinY = points.Select(p => p.Y).Min();
                var cMaxY = points.Select(p => p.Y).Max();
                rects.Add(
                    new Rectangle(
                        cMinX,
                        cMinY, 
                        cMaxX - cMinX, 
                        cMaxY - cMinY));
            }

            processingData.Add("RECTS", rects);

            var imageCandidateRects = rects.Where(r => r.Height > args.MaxLineHeight).ToList();

            if (!imageCandidateRects.Any()) return null;

            var minX = imageCandidateRects.Select(r => r.X).Min();
            var maxX = imageCandidateRects.Select(r => r.X + r.Width).Max();
            var minY = imageCandidateRects.Select(r => r.Y).Min();
            var maxY = imageCandidateRects.Select(r => r.Y + r.Height).Max();
            var imageRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);

            var nonImageRects = rects.Where(r => !r.IntersectsWith(imageRect)).ToArray();

            // expanding to the left
            var leftExpandedRectangle = new Rectangle(imageRect.Location, imageRect.Size);
            for (var i = 0; i < args.SideExpandMax; i++)
            {
                if (leftExpandedRectangle.X < 1) break;

                var seedRectangle = new Rectangle(
                    leftExpandedRectangle.X - 1, 
                    leftExpandedRectangle.Y, 
                    imageRect.Width + 1, 
                    imageRect.Height);

                if (nonImageRects.Any(r => r.IntersectsWith(seedRectangle))) break;

                leftExpandedRectangle = seedRectangle;
            }
            var imageRectNewX = (leftExpandedRectangle.X + imageRect.X) / 2;
            var imageRectNewWidth = imageRect.Width + imageRect.X - imageRectNewX;
            imageRect = new Rectangle(imageRectNewX, imageRect.Y, imageRectNewWidth, imageRect.Height);

            // expanding to the right
            var rightExpandedRectangle = new Rectangle(imageRect.Location, imageRect.Size);
            for (var i = 0; i < args.SideExpandMax; i++)
            {
                if (imageRect.X + imageRect.Width >= image.Width) break;

                var seedRectangle = new Rectangle(
                    rightExpandedRectangle.X, 
                    rightExpandedRectangle.Y, 
                    rightExpandedRectangle.Width + 1, 
                    rightExpandedRectangle.Height);

                if (nonImageRects.Any(r => r.IntersectsWith(seedRectangle))) break;

                rightExpandedRectangle = seedRectangle;
            }
            imageRectNewWidth = (imageRect.Width + rightExpandedRectangle.Width) / 2;
            imageRect = new Rectangle(imageRect.X, imageRect.Y, imageRectNewWidth, imageRect.Height);

            // expanding to the bottom
            var underImageRect = new Rectangle(
                imageRect.X,
                imageRect.Y + imageRect.Height,
                imageRect.Width,
                1000);
            var underImageTextRects = rects
                .Where(r => r.GetIntersectionWidth(underImageRect) > args.GeneralDelta)
                .Where(r => r.Width > imageRect.Width)
                .OrderBy(r => r.Y)
                .ToArray();
            if (underImageTextRects.Any())
            {
                imageRect.Height = underImageTextRects[0].Y - imageRect.Y - args.GeneralDelta;
            }
            else
            {
                var imageTitleAreaRect = new Rectangle(
                    imageRect.X,
                    imageRect.Y + imageRect.Height,
                    imageRect.Width,
                    args.ImageTitleAreaHeight);
                var imageTitleRects = rects
                    .Where(r => imageTitleAreaRect.Contains(r))
                    .OrderByDescending(r => r.Y)
                    .ToArray();
                if (imageTitleRects.Any())
                {
                    imageRect.Height = imageTitleRects[0].Y + imageTitleRects[0].Height - imageRect.Y + args.GeneralDelta;
                }
            }

            //var imageTitleAreaRect = new Rectangle(imageRect.X, imageRect.Y + imageRect.Height, imageRect.Width,
            //    args.ImageTitleAreaHeight);
            //var nonImageBottomRects = rects
            //    .Where(r => imageTitleAreaRect.Contains(r))
            //    .OrderByDescending(r => r.Y)
            //    .ToArray();
            //if (nonImageBottomRects.Any())
            //{
            //    imageRect.Height = nonImageBottomRects[0].Y + nonImageBottomRects[0].Height - imageRect.Y;
            //}

            return imageRect;
        }

        public static IEnumerable<UncoveredContour> GetUncoveredContours(string imageFile, OcrPage page)
        {
            var rects = GetSmoothedContourRectangles(imageFile).ToList();
            foreach (var rect in rects)
            {
                var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(rect));

                if (line == null) continue;

                var contour = new UncoveredContour
                {
                    ImageFile = imageFile,
                    Rectangle = rect,
                    Page = page,
                    Word = new OcrWord
                    {
                        TopLeftX = rect.X,
                        TopLeftY = rect.Y,
                        BottomRightX = rect.X + rect.Width,
                        BottomRightY = rect.Y + rect.Height,
                        LineBottomDistance = line.BottomRightY - rect.Y - rect.Height,
                        LineTopDistance = rect.Y - line.TopLeftY
                    }
                };

                yield return contour;
            }
        }

        private static bool CommentLinkMatch(Rectangle candidateRect, OcrPage page, CommentLinkSettings settings)
        {
            var line = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(candidateRect));

            if (line == null) return false;

            var lineBottomDistance = line.BottomRightY - candidateRect.Y - candidateRect.Height;

            if (lineBottomDistance > settings.MaxLineBottomDistance) return false;

            if (lineBottomDistance < settings.MinLineBottomDistance) return false;

            if (!settings.SizeMatch(candidateRect)) return false;

            return true;
        }
    }
}