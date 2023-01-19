using System.Drawing;
using System.Windows.Forms;

namespace LeninSearch.Studio.WinForms
{
    public static class PictureBoxExtensions
    {
        public static Rectangle ToOriginalRectangle(this PictureBox pictureBox, Rectangle pictureBoxRectangle)
        {
            var pbToOriginal = 1.0 * pictureBox.Image.Height / pictureBox.Height;
            var originalRectY = (int)(pictureBoxRectangle.Y * pbToOriginal);
            var pbLeftMargin = (pictureBox.Width - pictureBox.Image.Width / pbToOriginal) / 2;
            var originalRectX = (int)((pictureBoxRectangle.X - pbLeftMargin) * pbToOriginal);
            var originalRectWidth = (int)(pictureBoxRectangle.Size.Width * pbToOriginal);
            var originalRectHeight = (int)(pictureBoxRectangle.Size.Height * pbToOriginal);
            return new Rectangle(originalRectX, originalRectY, originalRectWidth, originalRectHeight);
        }

        public static Point ToOriginalPoint(this PictureBox pictureBox, Point pictureBoxPoint)
        {
            var pbToOriginal = 1.0 * pictureBox.Image.Height / pictureBox.Height;
            var originalY = (int)(pictureBoxPoint.Y * pbToOriginal);
            var pbLeftMargin = (pictureBox.Width - pictureBox.Image.Width / pbToOriginal) / 2;
            var originalX = (int)((pictureBoxPoint.X - pbLeftMargin) * pbToOriginal);
            return new Point(originalX, originalY);
        }

        public static Rectangle ToPictureBoxRectangle(this PictureBox pictureBox, Rectangle originalRectangle)
        {
            var originalToPb = 1.0 * pictureBox.Height / pictureBox.Image.Height;
            var pbRectY = (int)(originalRectangle.Y * originalToPb);
            var pbLeftMargin = (pictureBox.Width - pictureBox.Image.Width * originalToPb) / 2;
            var pbRectX = (int)(originalRectangle.X * originalToPb + pbLeftMargin);
            var pbRectWidth = (int)(originalRectangle.Size.Width * originalToPb);
            var pbRectHeight = (int)(originalRectangle.Size.Height * originalToPb);
            return new Rectangle(pbRectX, pbRectY, pbRectWidth, pbRectHeight);
        }

        public static Point ToPictureBoxPoint(this PictureBox pictureBox, Point originalPoint)
        {
            var originalToPb = 1.0 * pictureBox.Height / pictureBox.Image.Height;
            var pbY = (int)(originalPoint.Y * originalToPb);
            var pbLeftMargin = (pictureBox.Width - pictureBox.Image.Width * originalToPb) / 2;
            var pbX = (int)(originalPoint.X * originalToPb + pbLeftMargin);
            return new Point(pbX, pbY);
        }
    }
}