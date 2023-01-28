using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;
using BookProject.WinForms.Model;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace BookProject.WinForms.CV
{
    public static class CvUtil
    {
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

        public static IEnumerable<UncoveredContour> GetUncoveredContours(string imageFile, OldBookProjectPage page)
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
                    Word = new OldBookProjectWord
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

        private static bool CommentLinkMatch(Rectangle candidateRect, OldBookProjectPage page, CommentLinkSettings settings)
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