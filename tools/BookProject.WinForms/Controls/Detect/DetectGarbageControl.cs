using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Detectors;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.WinForms.Controls.Detect
{
    public partial class DetectGarbageControl : UserControl
    {
        private BookViewModel _bookVm;
        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;
            SetSettings(_bookVm.Settings.GarbageDetection);
        }

        private DetectGarbageSettings GetSettings()
        {
            return new DetectGarbageSettings
            {
                MinLeft = (int)minLeft_nud.Value,
                MinRight = (int)minRight_nud.Value,
                MinHeight = (int)minHeight_nud.Value,
                MaxHeight = (int)maxHeight_nud.Value,
                GaussSigma1 = (int)gaussSigma1_nud.Value,
                GaussSigma2 = (int)gaussSigma2_nud.Value,
                AddPadding = (int)addPadding_nud.Value
            };
        }
        private void SetSettings(DetectGarbageSettings settings)
        {
            minLeft_nud.Value = settings.MinLeft;
            minRight_nud.Value = settings.MinRight;
            minHeight_nud.Value = settings.MinHeight;
            maxHeight_nud.Value = settings.MaxHeight;
            gaussSigma1_nud.Value = settings.GaussSigma1;
            gaussSigma2_nud.Value = settings.GaussSigma2;
            addPadding_nud.Value = settings.AddPadding;
        }

        public DetectGarbageControl()
        {
            InitializeComponent();

            minLeft_nud.Minimum = 0;
            minLeft_nud.Maximum = 1000;
            minLeft_nud.Value = 10;

            minRight_nud.Minimum = 0;
            minRight_nud.Maximum = 1000;
            minRight_nud.Value = 10;

            minHeight_nud.Minimum = 1;
            minHeight_nud.Maximum = 1000;
            minHeight_nud.Value = 10;

            maxHeight_nud.Minimum = 1;
            maxHeight_nud.Maximum = 1000;
            maxHeight_nud.Value = 200;

            gaussSigma1_nud.Minimum = 1;
            gaussSigma1_nud.Maximum = 25;
            gaussSigma1_nud.Value = 16;

            gaussSigma2_nud.Minimum = 1;
            gaussSigma2_nud.Maximum = 25;
            gaussSigma2_nud.Value = 1;

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
            _bookVm.SetAndSaveDetectGarbageSettings(this, settings);
            MessageBox.Show("Saved!", "Garbage detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle)
                    .Concat(page.TitleBlocks.Select(b => b.Rectangle))
                    .Concat(page.CommentLinkBlocks.Select(b => b.Rectangle))
                    .ToArray();
                var garbageRects = new GarbageDetector()
                    .Detect(ImageUtility.Load(imageFile), settings, excludeRects, null);
                _bookVm.SetPageBlocks(this, page, garbageRects.Select(GarbageBlock.FromRectangle));
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            MessageBox.Show("Completed!", "Detect Garbage", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var renderer = new TestDetectGarbageImageRenderer(settings);
            _bookVm.SetImageRenderer(this, renderer);
        }
    }
}
