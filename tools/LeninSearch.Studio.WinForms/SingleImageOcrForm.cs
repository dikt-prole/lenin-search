using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LeninSearch.Studio.Core.Detectors;
using LeninSearch.Studio.Core.Models;
using LeninSearch.Studio.Core.Settings;
using LeninSearch.Studio.WinForms.CV;
using LeninSearch.Studio.WinForms.Model;
using LeninSearch.Studio.WinForms.Service;
using LeninSearch.Studio.WinForms.YandexVision;
using LeninSearch.Studio.WinForms.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Studio.WinForms
{
    public partial class SingleImageOcrForm : Form
    {
        private OcrPage _ocrPage;
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
            if (_ocrPage == null) return;

            var originalPoint = processed_pb.ToOriginalPoint(e.Location);

            var word = _ocrPage.Lines.Where(l => l.Words != null)
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
                            new YandexVisionOcrLineService()));

            var ocrResult = await ocrService.GetOcrPageAsync(file_tb.Text);
            _ocrPage = ocrResult.Page;

            var clCandidates = CvUtil.GetUncoveredContours(file_tb.Text, _ocrPage).ToList();

            var commentLinkWords = clCandidates.Select(c => c.Word).ToList();
            foreach (var clw in commentLinkWords) clw.IsCommentLinkNumber = true;

            // draw blocks on an image
            var image = new Bitmap(Image.FromFile(file_tb.Text));
            using var g = Graphics.FromImage(image);

            using var lineBrush = new SolidBrush(Color.FromArgb(100, Color.Red));
            using var commentLinkPen = new Pen(Color.Blue, 2);
            foreach (var line in _ocrPage.Lines)
            {
                g.FillRectangle(lineBrush, line.Rectangle);
            }

            foreach (var word in commentLinkWords)
            {
                var circleX = word.CenterX - OcrSettings.WordCircleRadius;
                var circleY = word.CenterY - OcrSettings.WordCircleRadius;
                var diameter = OcrSettings.WordCircleRadius * 2;
                g.DrawEllipse(commentLinkPen, circleX, circleY, diameter, diameter);
            }

            processed_pb.Image = image;
        }

        private void DrawBoundingBox(Graphics g, BoundingBox box, Color color)
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
