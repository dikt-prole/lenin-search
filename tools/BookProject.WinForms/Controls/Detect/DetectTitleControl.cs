using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BookProject.Core.Detectors;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.WinForms.Controls.Detect
{
    public partial class DetectTitleControl : UserControl
    {
        private BookViewModel _bookVm;
        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;
            SetSettings(_bookVm.Settings.TitleDetection);
        }

        private DetectTitleSettings GetSettings()
        {
            return new DetectTitleSettings
            {
                MinLeft = (int)minLeft_nud.Value,
                MinRight = (int)minRight_nud.Value,
                MinBottom = (int)minBottom_nud.Value,
                MinTop = (int)minTop_nud.Value,
                GaussSigma1 = (int)gaussSigma1_nud.Value,
                GaussSigma2 = (int)gaussSigma2_nud.Value,
                MaxLineDist = (int)maxLineDist_nud.Value,
                AddPadding = (int)addPadding_nud.Value
            };
        }

        private void SetSettings(DetectTitleSettings settings)
        {
            minLeft_nud.Value = settings.MinLeft;
            minRight_nud.Value = settings.MinRight;
            minTop_nud.Value = settings.MinTop;
            minBottom_nud.Value = settings.MinBottom;
            gaussSigma1_nud.Value = settings.GaussSigma1;
            gaussSigma2_nud.Value = settings.GaussSigma2;
            maxLineDist_nud.Value = settings.MaxLineDist;
            addPadding_nud.Value = settings.AddPadding;
        }

        public DetectTitleControl()
        {
            InitializeComponent();

            minLeft_nud.Minimum = 0;
            minLeft_nud.Maximum = 1000;
            minLeft_nud.Value = 10;

            minRight_nud.Minimum = 0;
            minRight_nud.Maximum = 1000;
            minRight_nud.Value = 10;

            minTop_nud.Minimum = 0;
            minTop_nud.Maximum = 1000;
            minTop_nud.Value = 10;

            minBottom_nud.Minimum = 0;
            minBottom_nud.Maximum = 1000;
            minBottom_nud.Value = 10;

            gaussSigma1_nud.Minimum = 1;
            gaussSigma1_nud.Maximum = 25;
            gaussSigma1_nud.Value = 4;

            gaussSigma2_nud.Minimum = 1;
            gaussSigma2_nud.Maximum = 25;
            gaussSigma2_nud.Value = 4;

            maxLineDist_nud.Minimum = 0;
            maxLineDist_nud.Maximum = 500;
            maxLineDist_nud.Value = 10;

            addPadding_nud.Minimum = 0;
            addPadding_nud.Maximum = 50;
            addPadding_nud.Value = 5;

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
            _bookVm.SetAndSaveDetectTitleSettings(this, settings);
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
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle).ToArray();
                using var image = ImageUtility.Load(imageFile);
                var titleRects = new TitleDetector().Detect(image, settings, excludeRects, null);
                var titleBlocks = titleRects.Select(TitleBlock.FromRectangle).ToArray();
                foreach (var titleBlock in titleBlocks)
                {
                    using var ocrImage = ImageUtility.Crop(image, titleBlock.Rectangle);
                    using var ocrImageStream = new MemoryStream();
                    ocrImage.Save(ocrImageStream, ImageFormat.Jpeg);
                    var ocrImageBytes = ocrImageStream.ToArray();
                    var ocrPageResult = Task.Run(() => _bookVm.OcrUtility.GetPageAsync(ocrImageBytes)).Result;
                    titleBlock.Text = ocrPageResult.GetText();
                }

                _bookVm.SetPageBlocks(this, page, titleBlocks);
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

            var settings = GetSettings();
            var renderer = new TestDetectTitleImageRenderer(settings);
            _bookVm.SetImageRenderer(this, renderer);
        }
    }
}
