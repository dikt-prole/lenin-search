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
    public partial class DetectCommentLinkNumberControl : UserControl
    {
        private BookViewModel _bookVm;

        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;
            SetSettings(_bookVm.Settings.CommentLinkDetection);
        }

        private DetectCommentLinkSettings GetSettings()
        {
            return new DetectCommentLinkSettings
            {
                MinWidth = (int)minWidth_nud.Value,
                MinHeight = (int)minHeight_nud.Value,
                MaxHeight = (int)maxHeight_nud.Value,
                MaxWidth = (int)maxWidth_nud.Value,
                LinkGaussSigma1 = (int)linkGaussSigma1_nud.Value,
                LinkGaussSigma2 = (int)linkGaussSigma2_nud.Value,
                LineGaussSigma1 = (int)lineGaussSigma1_nud.Value,
                LineGaussSigma2 = (int)lineGaussSigma2_nud.Value,
                LineHeightPartMax = (double)lineHeightPartMax_nud.Value,
                LineTopDistanceMax = (int)lineTopDistMax_nud.Value,
                AllowedSymbols = allowedSymbols_tb.Text,
                AddPadding = (int)addPadding_nud.Value
            };
        }

        private void SetSettings(DetectCommentLinkSettings settings)
        {
            minWidth_nud.Value = settings.MinWidth;
            minHeight_nud.Value = settings.MinHeight;
            maxWidth_nud.Value = settings.MaxWidth;
            maxHeight_nud.Value = settings.MaxHeight;
            linkGaussSigma1_nud.Value = settings.LinkGaussSigma1;
            linkGaussSigma2_nud.Value = settings.LinkGaussSigma2;
            lineGaussSigma1_nud.Value = settings.LineGaussSigma1;
            lineGaussSigma2_nud.Value = settings.LineGaussSigma2;
            lineHeightPartMax_nud.Value = (decimal)settings.LineHeightPartMax;
            lineTopDistMax_nud.Value = settings.LineTopDistanceMax;
            allowedSymbols_tb.Text = settings.AllowedSymbols;
            addPadding_nud.Value = settings.AddPadding;
        }

        public DetectCommentLinkNumberControl()
        {
            InitializeComponent();

            minWidth_nud.Minimum = 1;
            minWidth_nud.Maximum = 50;
            minWidth_nud.Value = 12;

            minHeight_nud.Minimum = 1;
            minHeight_nud.Maximum = 50;
            minHeight_nud.Value = 12;

            maxWidth_nud.Minimum = 1;
            maxWidth_nud.Maximum = 100;
            maxWidth_nud.Value = 20;

            maxHeight_nud.Minimum = 1;
            maxHeight_nud.Maximum = 100;
            maxHeight_nud.Value = 20;

            linkGaussSigma1_nud.Minimum = 0;
            linkGaussSigma1_nud.Maximum = 25;
            linkGaussSigma1_nud.Value = 1;

            linkGaussSigma2_nud.Minimum = 0;
            linkGaussSigma2_nud.Maximum = 25;
            linkGaussSigma2_nud.Value = 1;

            lineGaussSigma1_nud.Minimum = 0;
            lineGaussSigma1_nud.Maximum = 25;
            lineGaussSigma1_nud.Value = 16;

            lineGaussSigma2_nud.Minimum = 0;
            lineGaussSigma2_nud.Maximum = 25;
            lineGaussSigma2_nud.Value = 1;

            lineHeightPartMax_nud.DecimalPlaces = 2;
            lineHeightPartMax_nud.Increment = (decimal)0.1;
            lineHeightPartMax_nud.Minimum = (decimal)0.25;
            lineHeightPartMax_nud.Maximum = 1;
            lineHeightPartMax_nud.Value = (decimal)0.75;

            lineTopDistMax_nud.Minimum = 0;
            lineTopDistMax_nud.Maximum = 25;
            lineTopDistMax_nud.Value = 2;

            addPadding_nud.Minimum = 0;
            addPadding_nud.Maximum = 100;
            addPadding_nud.Value = 2;

            allowedSymbols_tb.Text = "*";

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
            _bookVm.SetAndSaveDetectCommentLinkSettings(this, settings);
            MessageBox.Show("Saved!", "Comment link detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    .ToArray();
                var commentLinkRects = new CommentLinkDetector(new YandexVisionOcrUtility())
                    .Detect(ImageUtility.Load(imageFile), settings, excludeRects, null);
                _bookVm.SetPageBlocks(this, page, commentLinkRects.Select(GarbageBlock.FromRectangle));
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            MessageBox.Show("Completed!", "Detect Comment Links", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            var renderer = new TestDetectCommentLinkNumberImageRenderer(settings, _bookVm.OcrUtility);
            _bookVm.SetImageRenderer(this, renderer);
        }
    }
}
