using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
{
    public partial class CommentLinkControl : UserControl
    {
        public string WordText => word_tb.Text;

        private CommentLinkCandidate _contour;
        public CommentLinkCandidate Contour
        {
            get => _contour;
            set
            {
                _contour = value;
                word_tb.Text = _contour.Word.Text;
                using var image = new Bitmap(Image.FromFile(_contour.ImageFile));
                using var g = Graphics.FromImage(image);
                g.DrawRectangle(Pens.Red, _contour.Rectangle);

                using var testImage = image.Clone(_contour.Rectangle, image.PixelFormat);
                var testImagePath = $"D:\\Repo\\lenin-search\\corpus\\playground\\{Guid.NewGuid()}.jpg";
                testImage.Save(testImagePath, ImageFormat.Jpeg);

                var margin = _contour.Rectangle.Height > rectangle_pb.Height ? 0 : (rectangle_pb.Height - _contour.Rectangle.Height) / 2;
                var lineRect = new Rectangle(0, _contour.Rectangle.Y - margin, image.Width, _contour.Rectangle.Height + margin * 2);
                var lineImage = image.Clone(lineRect, image.PixelFormat);
                rectangle_pb.Width = lineImage.Width;
                rectangle_pb.Image = lineImage;
                size_lbl.Text = $"w: {_contour.Rectangle.Width}, h: {_contour.Rectangle.Height}, lbd: {_contour.Word.LineBottomDistance:F2}";
            }
        }

        public CommentLinkControl()
        {
            InitializeComponent();
        }
    }
}
