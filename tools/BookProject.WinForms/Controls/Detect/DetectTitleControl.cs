using System;
using System.Windows.Forms;
using BookProject.Core.Settings;

namespace BookProject.WinForms.Controls.Detect
{
    public partial class DetectTitleControl : UserControl
    {
        public event EventHandler TestStart;

        public event EventHandler TestEnd;

        public event EventHandler Detect;

        public event EventHandler Save;
        public DetectTitleSettings GetSettings()
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

        public void SetSettings(DetectTitleSettings settings)
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

            test_btn.MouseDown += (sender, args) => TestStart?.Invoke(this, EventArgs.Empty);
            test_btn.MouseUp += (sender, args) => TestEnd?.Invoke(this, EventArgs.Empty);
            detect_btn.Click += (sender, args) => Detect?.Invoke(this, EventArgs.Empty);
            save_btn.Click += (sender, args) => Save?.Invoke(this, EventArgs.Empty);
        }
    }
}
