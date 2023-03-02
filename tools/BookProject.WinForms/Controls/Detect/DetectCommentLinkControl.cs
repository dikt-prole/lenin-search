using System;
using System.Collections.Generic;
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
    public partial class DetectCommentLinkControl : UserControl
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
                LineGaussSigma1 = (int)lineGaussSigma1_nud.Value,
                LineGaussSigma2 = (int)lineGaussSigma2_nud.Value,
                TopDeltaMax = (double)topDeltaMax_nud.Value,
                BottomDeltaMin = (int)bottomDeltaMin_nud.Value,
                AddPadding = (int)addPadding_nud.Value
            };
        }

        private void SetSettings(DetectCommentLinkSettings settings)
        {
            lineGaussSigma1_nud.Value = settings.LineGaussSigma1;
            lineGaussSigma2_nud.Value = settings.LineGaussSigma2;
            topDeltaMax_nud.Value = (decimal)settings.TopDeltaMax;
            bottomDeltaMin_nud.Value = settings.BottomDeltaMin;
            addPadding_nud.Value = settings.AddPadding;
        }

        public DetectCommentLinkControl()
        {
            InitializeComponent();

            lineGaussSigma1_nud.Minimum = 0;
            lineGaussSigma1_nud.Maximum = 25;
            lineGaussSigma1_nud.Value = 16;

            lineGaussSigma2_nud.Minimum = 0;
            lineGaussSigma2_nud.Maximum = 25;
            lineGaussSigma2_nud.Value = 1;

            topDeltaMax_nud.DecimalPlaces = 2;
            topDeltaMax_nud.Increment = (decimal)0.01;
            topDeltaMax_nud.Minimum = (decimal)0.05;
            topDeltaMax_nud.Maximum = 1;
            topDeltaMax_nud.Value = (decimal)0.15;

            bottomDeltaMin_nud.Minimum = 0;
            bottomDeltaMin_nud.Maximum = 100;
            bottomDeltaMin_nud.Value = 5;

            addPadding_nud.Minimum = 0;
            addPadding_nud.Maximum = 100;
            addPadding_nud.Value = 2;

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

            _bookVm.SendInfo(this, "Started comment link detection");

            for (var i = 0; i < imageFiles.Length; i++)
            {
                var imageFile = imageFiles[i];
                var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle)
                    .ToArray();
                var commentLinkRects = new CommentLinkDetector(new CvUtility())
                    .Detect(ImageUtility.Load(imageFile), settings, excludeRects, null);

                var commentLinkBlocks = new List<CommentLinkBlock>();
                using var image = ImageUtility.Load(imageFile);
                foreach (var commentLinkRect in commentLinkRects)
                {
                    var commentLinkBlock = CommentLinkBlock.FromRectangle(commentLinkRect);

                    using var commentLinkBitmap = ImageUtility.Crop(image, commentLinkRect);
                    var commentLinkBytes = ImageUtility.GetJpegBytes(commentLinkBitmap);
                    commentLinkBlock.CommentText =
                        Task.Run(() => _bookVm.OcrUtility.GetPageAsync(commentLinkBytes)).Result.GetText();
                    commentLinkBlocks.Add(commentLinkBlock);
                }

                _bookVm.SetPageBlocks(this, page, commentLinkBlocks);
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            _bookVm.SendInfo(this, "Comment link detection complete");
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
            var renderer = new TestDetectCommentLinkNumberImageRenderer(new CommentLinkDetector(new CvUtility()), settings);
            _bookVm.SetImageRenderer(this, renderer);
        }
    }
}
