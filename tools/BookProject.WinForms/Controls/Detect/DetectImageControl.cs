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
    public partial class DetectImageControl : UserControl
    {
        private BookViewModel _bookVm;
        private DetectImageSettings GetSettings()
        {
            return new DetectImageSettings
            {
                MinLeft = (int)minLeft_nud.Value,
                MinRight = (int)minRight_nud.Value,
                MinBottom = (int)minBottom_nud.Value,
                MinTop = (int)minTop_nud.Value,
                GaussSigma1 = (int)gaussSigma1_nud.Value,
                GaussSigma2 = (int)gaussSigma2_nud.Value,
                MinHeight = (int)minHight_nud.Value,
                AddPadding = (int)addPadding_nud.Value
            };
        }

        private void SetSettings(DetectImageSettings settings)
        {
            minLeft_nud.Value = settings.MinLeft;
            minRight_nud.Value = settings.MinRight;
            minTop_nud.Value = settings.MinTop;
            minBottom_nud.Value = settings.MinBottom;
            gaussSigma1_nud.Value = settings.GaussSigma1;
            gaussSigma2_nud.Value = settings.GaussSigma2;
            minHight_nud.Value = settings.MinHeight;
            addPadding_nud.Value = settings.AddPadding;
        }

        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;
            SetSettings(_bookVm.Settings.ImageDetection);
        }

        public DetectImageControl()
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

            minHight_nud.Minimum = 10;
            minHight_nud.Maximum = 500;
            minHight_nud.Value = 100;

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
            if (_bookVm != null)
            {
                var settings = GetSettings();
                _bookVm.SetAndSaveDetectImageSettings(this, settings);
                MessageBox.Show("Saved!", "Image detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DetectBtnOnClick(object sender, EventArgs e)
        {
            if (_bookVm != null)
            {
                var dialog = new ImageScopeDialog();
                if (dialog.ShowDialog() != DialogResult.OK) return;

                var imageFiles = dialog.GetMatchingImages(_bookVm.Book.Folder);

                var settings = GetSettings();
                progressBar1.Minimum = 0;
                progressBar1.Maximum = imageFiles.Length;
                progressBar1.Value = 0;

                for (var i = 0; i < imageFiles.Length; i++)
                {
                    var imageFile = imageFiles[i];
                    var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                    var imageRects = new ImageDetector().Detect(ImageUtility.Load(imageFile), settings, null, null);
                    _bookVm.SetPageBlocks(this, page, imageRects.Select(ImageBlock.FromRectangle));
                    progressBar1.Value = i + 1;
                    Application.DoEvents();
                }

                MessageBox.Show("Completed!", "Detect Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar1.Value = 0;
            }
        }

        private void TestBtnOnMouseUp(object sender, MouseEventArgs e)
        {
            if (_bookVm != null)
            {
                _bookVm.SetImageRenderer(this, new PageStateRenderer(_bookVm));
            }
        }

        private void TestBtnOnMouseDown(object sender, MouseEventArgs e)
        {
            if (_bookVm != null)
            {
                _bookVm.SetImageRenderer(this, new TestDetectImageImageRenderer(GetSettings()));
            }
        }
    }
}
