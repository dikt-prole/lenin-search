using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.WinForms.Controls.Detect
{
    public partial class DetectLineControl : UserControl
    {
        private BookViewModel _bookVm;
        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;
            SetSettings(_bookVm.Settings.LineDetection);
        }

        private DetectLineSettings GetSettings()
        {
            return new DetectLineSettings
            {
                MinIndent = (int)minIndent_nud.Value
            };
        }

        private void SetSettings(DetectLineSettings settings)
        {
            minIndent_nud.Value = settings?.MinIndent ?? 0;
        }

        public DetectLineControl()
        {
            InitializeComponent();

            minIndent_nud.Minimum = 0;
            minIndent_nud.Maximum = 200;
            minIndent_nud.Value = 20;

            test_btn.MouseDown += TestBtnOnMouseDown;
            test_btn.MouseUp += TestBtnOnMouseUp;
            detect_btn.Click += DetectBtnOnClick;
            save_btn.Click += SaveBtnOnClick;

            progressBar1.Value = 0;
        }

        private void SaveBtnOnClick(object sender, EventArgs e)
        {
            if (_bookVm == null) return;

            var settings = GetSettings();
            _bookVm.SetAndSaveDetectLineSettings(this, settings);
            MessageBox.Show("Saved!", "Title detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DetectBtnOnClick(object sender, EventArgs e)
        {
            if (_bookVm == null) return;

            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = dialog.GetMatchingImages(_bookVm.Book.Folder);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Length;
            progressBar1.Value = 0;
            var settings = GetSettings();

            for (var i = 0; i < imageFiles.Length; i++)
            {
                var imageFile = imageFiles[i];
                var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                using var sourceImage = ImageUtility.Load(imageFile);
                var excludeRectangles = page.ImageBlocks
                    .Concat<Block>(page.TitleBlocks)
                    .Concat(page.CommentLinkBlocks)
                    .Concat(page.GarbageBlocks)
                    .Select(b => b.Rectangle)
                    .ToArray();
                using var cleanImage = ImageUtility.WhiteOut(sourceImage, excludeRectangles);
                var cleanImageBytes = ImageUtility.GetJpegBytes(cleanImage);
                var ocrPage = Task.Run(() => _bookVm.OcrUtility.GetPageAsync(cleanImageBytes)).Result;

                if (ocrPage.Lines?.Any() != true) continue;

                var lines = ocrPage.Lines
                    .Select(l => l.ToLine())
                    .OrderBy(l => l.TopLeftY)
                    .ToArray();

                for (var j = 1; j < lines.Length; j++)
                {
                    var currX = lines[j].TopLeftX;
                    var prevX = lines[j - 1].TopLeftX;
                    if (prevX - currX > settings.MinIndent)
                    {
                        lines[j - 1].Type = LineType.First;
                    }
                }

                _bookVm.SetPageBlocks(this, page, lines);
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            MessageBox.Show("Completed!", "Detect Titles", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void TestBtnOnMouseUp(object sender, MouseEventArgs e)
        {
            if (_bookVm == null) return;

            var renderer = new PageStateRenderer(_bookVm);
            _bookVm.SetImageRenderer(this, renderer);
        }

        private void TestBtnOnMouseDown(object sender, MouseEventArgs e)
        {
            if (_bookVm == null) return;

            /*
            var settings = GetSettings();
            var renderer = new TestDetectTitleImageRenderer(settings);
            _bookVm.SetImageRenderer(this, renderer);
            */
        }
    }
}
