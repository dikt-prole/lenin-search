using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Detectors;
using BookProject.Core.Models.Book.Old;
using BookProject.Core.Models.YandexVision.Response;
using BookProject.Core.Settings;
using BookProject.WinForms.CV;
using BookProject.WinForms.Model;
using Newtonsoft.Json;

namespace BookProject.WinForms
{
    public partial class SingleImageOcrForm : Form
    {
        private OldBookProjectPage _bookProjectPage;
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
           
        }

        private async void Ocr_btnOnClick(object? sender, EventArgs e)
        {
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
