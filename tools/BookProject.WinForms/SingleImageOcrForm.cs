using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Detectors;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.YandexVision.Response;
using BookProject.Core.Settings;
using BookProject.WinForms.CV;
using BookProject.WinForms.Model;
using BookProject.WinForms.Service;
using BookProject.WinForms.YandexVision;
using Newtonsoft.Json;

namespace BookProject.WinForms
{
    public partial class SingleImageOcrForm : Form
    {
        private BookProjectPage _bookProjectPage;
        public SingleImageOcrForm()
        {
            InitializeComponent();
            load_btn.Click += Load_btnOnClick;
            ocr_btn.Click += Ocr_btnOnClick;
            test_btn.Click += TestClick;
            prev_btn.Click += Prev_btnOnClick;
            next_btn.Click += Next_btnOnClick;
            processed_pb.MouseDown += ProcessedOnMouseDown;
        }

        private void ProcessedOnMouseDown(object sender, MouseEventArgs e)
        {
            if (_bookProjectPage == null) return;

            var originalPoint = processed_pb.ToOriginalPoint(e.Location);

            var word = _bookProjectPage.Lines.Where(l => l.Words != null)
                .SelectMany(l => l.Words)
                .FirstOrDefault(w => w.IsCommentLinkNumber && w.IsInsideWordCircle(originalPoint));

            if (word == null) return;

            var message = JsonConvert.SerializeObject(word, Formatting.Indented);

            MessageBox.Show(message, "Word info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Next_btnOnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_tb.Text)) return;

            var directory = Path.GetDirectoryName(file_tb.Text);

            var files = Directory.GetFiles(directory).Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")).ToList();

            var currentIndex = files.IndexOf(file_tb.Text);

            if (currentIndex < files.Count - 1)
            {
                file_tb.Text = files[currentIndex + 1];
                initial_pb.Image = Image.FromFile(file_tb.Text);
            }
        }

        private void Prev_btnOnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_tb.Text)) return;

            var directory = Path.GetDirectoryName(file_tb.Text);

            var files = Directory.GetFiles(directory).Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")).ToList();

            var currentIndex = files.IndexOf(file_tb.Text);

            if (currentIndex > 0)
            {
                file_tb.Text = files[currentIndex - 1];
                initial_pb.Image = Image.FromFile(file_tb.Text);
            }
        }

        private void TestClick(object? sender, EventArgs e)
        {
            var settings = new DetectTitleSettings
            {
                GaussSigma1 = 12,
                GaussSigma2 = 12,
                MinBottom = 100,
                MinTop = 100,
                MinRight = 160,
                MinLeft = 160
            };
            var internalValues = new Dictionary<string, object>();
            var titleDetector = new TitleDetector();
            var titleRectangles = titleDetector.Detect(file_tb.Text, settings, null, internalValues);
            var resultImage = internalValues["invertedGray"] as Bitmap;
            using var pen = new Pen(Color.Red, 4);
            using var g = Graphics.FromImage(resultImage);
            foreach (var titleRectangle in titleRectangles)
            {
                g.DrawRectangle(pen, titleRectangle);
            }
            processed_pb.Image = resultImage;
        }

        private async void Ocr_btnOnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_tb.Text)) return;

            var clSettings = new CommentLinkSettings
            {
                MinWidth = 3,
                MaxWidth = 10,
                MinHeight = 9,
                MaxHeight = 15,
                MinLineBottomDistance = 5,
                MaxLineBottomDistance = 15,
                MaxLineTopDistance = 3,
                MinLineTopDistance = -6
            };
            var ocrService =
                new FeatureSettingDecorator(
                    new IntersectingLineMergerDecorator(
                            new YandexVisionPageProviderService()));

            var ocrResult = await ocrService.GetOcrPageAsync(file_tb.Text);
            _bookProjectPage = ocrResult.Page;

            var clCandidates = CvUtil.GetUncoveredContours(file_tb.Text, _bookProjectPage).ToList();

            var commentLinkWords = clCandidates.Select(c => c.Word).ToList();
            foreach (var clw in commentLinkWords) clw.IsCommentLinkNumber = true;

            // draw blocks on an image
            var image = new Bitmap(Image.FromFile(file_tb.Text));
            using var g = Graphics.FromImage(image);

            using var lineBrush = new SolidBrush(Color.FromArgb(100, Color.Red));
            using var commentLinkPen = new Pen(Color.Blue, 2);
            foreach (var line in _bookProjectPage.Lines)
            {
                g.FillRectangle(lineBrush, line.Rectangle);
            }

            foreach (var word in commentLinkWords)
            {
                var circleX = word.CenterX - BookProjectSettings.WordCircleRadius;
                var circleY = word.CenterY - BookProjectSettings.WordCircleRadius;
                var diameter = BookProjectSettings.WordCircleRadius * 2;
                g.DrawEllipse(commentLinkPen, circleX, circleY, diameter, diameter);
            }

            processed_pb.Image = image;
        }

        private void DrawBoundingBox(Graphics g, YandexVisionBoundingBox box, Color color)
        {
            var pen = new Pen(color);

            var leftTop = box.Vertices[0];
            var leftBottom = box.Vertices[1];
            var rightBottom = box.Vertices[2];
            var rightTop = box.Vertices[3];

            g.DrawLine(pen, leftTop.Point(), leftBottom.Point());
            g.DrawLine(pen, leftBottom.Point(), rightBottom.Point());
            g.DrawLine(pen, rightBottom.Point(), rightTop.Point());
            g.DrawLine(pen, rightTop.Point(), leftTop.Point());
        }

        private void Load_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Open Image File",
                Filter = "images|*.jpeg;*.png;*.jpg"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            file_tb.Text = dialog.FileName;

            initial_pb.Image = Image.FromFile(file_tb.Text);
        }
    }
}
