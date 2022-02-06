using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using LeninSearch.Ocr.CV;
using LeninSearch.Ocr.Service;
using LeninSearch.Ocr.YandexVision;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Ocr
{
    public partial class SingleImageOcrForm : Form
    {
        public SingleImageOcrForm()
        {
            InitializeComponent();
            load_btn.Click += Load_btnOnClick;
            ocr_btn.Click += Ocr_btnOnClick;
            test_btn.Click += TestClick;
            prev_btn.Click += Prev_btnOnClick;
            next_btn.Click += Next_btnOnClick;
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
                pictureBox1.Image = Image.FromFile(file_tb.Text);
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
                pictureBox1.Image = Image.FromFile(file_tb.Text);
            }
        }

        private void TestClick(object? sender, EventArgs e)
        {
            var rects = CvUtil.GetContourRectangles(file_tb.Text).ToList();
            var image = new Bitmap(Image.FromFile(file_tb.Text));
            using var g = Graphics.FromImage(image);

            foreach (var rect in rects) g.DrawRectangle(Pens.Red, rect);

            pictureBox1.Image = image;
        }

        private async void Ocr_btnOnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_tb.Text)) return;

            IOcrService lineService = new YandexVisionOcrLineService();
            lineService = new UncoveredWordsDecorator(lineService);
            lineService = new IntersectingLineMergerDecorator(lineService);

            var ocrResult = await lineService.GetOcrPageAsync(file_tb.Text);
            var page = ocrResult.Page;

            // draw blocks on an image
            var image = new Bitmap(Image.FromFile(file_tb.Text));
            using var g = Graphics.FromImage(image);

            var color = Color.FromArgb(100, Color.Red);
            using var brush = new SolidBrush(color);
            foreach (var line in page.Lines)
            {
                g.FillRectangle(brush, line.Rectangle);
            }

            pictureBox1.Image = image;
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

            pictureBox1.Image = Image.FromFile(file_tb.Text);
        }
    }
}
