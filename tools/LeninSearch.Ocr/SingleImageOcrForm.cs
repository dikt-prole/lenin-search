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
using LeninSearch.Ocr.YandexVision;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Ocr
{
    public partial class SingleImageOcrForm : Form
    {
        private string _iamToken;
        private readonly HttpClient _httpClient = new HttpClient();

        public SingleImageOcrForm()
        {
            InitializeComponent();
            load_btn.Click += Load_btnOnClick;
            ocr_btn.Click += Ocr_btnOnClick;
            lines_btn.Click += Lines_btnOnClick;
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

        private void Lines_btnOnClick(object? sender, EventArgs e)
        {
            return;
        }

        private async void Ocr_btnOnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(file_tb.Text)) return;

            var lineService = new YandexVisionOcrLineService();

            var ocrResult = await lineService.GetOcrPageAsync(file_tb.Text);
            var page = ocrResult.Page;

            // draw blocks on an image
            var image = new Bitmap(Image.FromFile(file_tb.Text));
            using var g = Graphics.FromImage(image);

            foreach (var line in page.Lines)
            {
                g.DrawLine(Pens.Red, 0, line.TopLeftY, image.Width, line.TopLeftY);
                g.DrawLine(Pens.Red, 0, line.BottomRightY, image.Width, line.BottomRightY);
            }

            using var pen = new Pen(Color.DarkViolet, 4);

            g.DrawLine(pen, page.BottomDivider.LeftX, page.BottomDivider.Y, page.BottomDivider.RightX, page.BottomDivider.Y);
            g.DrawLine(pen, page.TopDivider.LeftX, page.TopDivider.Y, page.TopDivider.RightX, page.TopDivider.Y);


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

        private void RefreshIamToken()
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = @"C:\Users\vbncmx\yandex-cloud\bin\yc.exe",
                    Arguments = "iam create-token",
                    CreateNoWindow = true
                }
            };
            process.Start();
            _iamToken = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
    }
}
