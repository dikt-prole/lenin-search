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
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Ocr
{
    public partial class SingleImageOcrForm : Form
    {
        private string _imageFile;
        private string _iamToken;
        private readonly HttpClient _httpClient = new HttpClient();

        public SingleImageOcrForm()
        {
            InitializeComponent();
            load_btn.Click += Load_btnOnClick;
            ocr_btn.Click += Ocr_btnOnClick;
            lines_btn.Click += Lines_btnOnClick;
        }

        private void Lines_btnOnClick(object? sender, EventArgs e)
        {
            var dividerLines = CvUtil.GetTopBottomDividerLines(_imageFile);

            var tl = dividerLines.TopLine;
            var bl = dividerLines.BottomLine;

            var image = new Bitmap(Image.FromFile(_imageFile));

            using var g = Graphics.FromImage(image);

            g.DrawLine(Pens.Red, tl.LeftX, tl.Y, tl.RightX, tl.Y);
            g.DrawLine(Pens.Red, bl.LeftX, bl.Y, bl.RightX, bl.Y);

            pictureBox1.Image = image;
        }

        private int GetMinDistance(LineSegment2D l1, LineSegment2D l2)
        {
            var distances = new List<int>
            {
                Math.Abs(l1.P1.X - l2.P1.X),
                Math.Abs(l1.P1.X - l2.P2.X),
                Math.Abs(l1.P2.X - l2.P2.X),
                Math.Abs(l1.P2.X - l2.P1.X)
            };

            return distances.Min();
        }

        private async void Ocr_btnOnClick(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_imageFile)) return;

            if (string.IsNullOrEmpty(_iamToken)) RefreshIamToken();

            var imageBytes = File.ReadAllBytes(_imageFile);

            var ocrRequest = YtVisionRequest.Ocr(imageBytes);

            var ocrRequestJson = JsonConvert.SerializeObject(ocrRequest, Formatting.Indented);

            var apiKey = Environment.GetEnvironmentVariable("YandexApiKey");

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://vision.api.cloud.yandex.net/vision/v1/batchAnalyze"),
                Method = HttpMethod.Post,
                Headers =
                {
                    {HttpRequestHeader.ContentType.ToString(), "application/json"},
                    {HttpRequestHeader.Authorization.ToString(), $"Api-Key {apiKey}"}
                },
                Content = new StringContent(ocrRequestJson)
            };

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show($"Response code: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(responseJson);

            var blocks = ocrResponse.Results[0].Results[0].TextDetection.Pages[0].Blocks;

            // draw blocks on an image
            var image = new Bitmap(Image.FromFile(_imageFile));
            using (var g = Graphics.FromImage(image))
            {
                foreach (var block in blocks)
                {
                    foreach (var line in block.Lines)
                    {
                        DrawBoundingBox(g, line.BoundingBox, Color.Red);
                    }

                    DrawBoundingBox(g, block.BoundingBox, Color.Blue);
                }
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

            _imageFile = dialog.FileName;


            pictureBox1.Image = Image.FromFile(_imageFile);
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
