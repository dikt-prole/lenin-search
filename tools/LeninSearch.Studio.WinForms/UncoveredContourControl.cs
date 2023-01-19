using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LeninSearch.Studio.WinForms.Model;
using Bitmap = System.Drawing.Bitmap;

namespace LeninSearch.Studio.WinForms
{
    public partial class UncoveredContourControl : UserControl
    {
        public event EventHandler<Keys> Navigation;

        public string WordText => word_tb.Text;
        private UncoveredContour _contour;

        public UncoveredContour Contour
        {
            get => _contour;
            set
            {
                _contour = value;
                word_tb.Text = _contour.Word.Text;
                using var image = new Bitmap(Image.FromFile(_contour.ImageFile));
                using var g = Graphics.FromImage(image);

                // draw opaque ellipse
                var circleDiameter = Math.Max(_contour.Rectangle.Width, _contour.Rectangle.Height) + 6;
                var circleX = _contour.Rectangle.X + _contour.Rectangle.Width / 2 - circleDiameter / 2;
                var circleY = _contour.Rectangle.Y + _contour.Rectangle.Height / 2 - circleDiameter / 2;
                var circleColor = Color.FromArgb(30, 255, 0, 0);
                using var circleBrush = new SolidBrush(circleColor);
                g.FillEllipse(circleBrush, circleX, circleY, circleDiameter, circleDiameter);

                // draw contour border
                using var contourBorderPen = new Pen(Color.Red, 1);
                g.DrawRectangle(contourBorderPen, _contour.Rectangle);

                var margin = _contour.Rectangle.Height > rectangle_pb.Height ? 0 : (rectangle_pb.Height - _contour.Rectangle.Height) / 2;
                var lineRect = new Rectangle(0, _contour.Rectangle.Y - margin, image.Width, _contour.Rectangle.Height + margin * 2);
                var lineImage = image.Clone(lineRect, image.PixelFormat);
                rectangle_pb.Width = lineImage.Width;
                rectangle_pb.Image = lineImage;
                size_lbl.Text = $"w: {_contour.Rectangle.Width}, h: {_contour.Rectangle.Height}, lbd: {_contour.Word.LineBottomDistance:F2}";
            }
        }

        public UncoveredContourControl()
        {
            InitializeComponent();
            var trackedKeys = new [] { Keys.Up, Keys.Down, Keys.Left, Keys.Right };
            word_tb.KeyUp += (sender, args) =>
            {
                if (!trackedKeys.Contains(args.KeyCode)) return;
                Navigation?.Invoke(this, args.KeyCode);
            };
        }

        public void FocusWordTextBox()
        {
            word_tb.Focus();
        }
    }
}
