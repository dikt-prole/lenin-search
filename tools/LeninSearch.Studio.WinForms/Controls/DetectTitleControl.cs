using System;
using System.Windows.Forms;
using LeninSearch.Studio.Core.Settings;

namespace LeninSearch.Studio.WinForms.Controls
{
    public partial class DetectTitleControl : UserControl
    {
        public event EventHandler TestStart;

        public event EventHandler TestEnd;

        public event EventHandler Apply;

        public DetectTitleSettings Settings
        {
            get => new DetectTitleSettings
            {
                MinLeft = (int)minLeft_nud.Value,
                MinRight = (int)minRight_nud.Value,
                MinBottom = (int)minBottom_nud.Value,
                MinTop = (int)minTop_nud.Value,
                GaussSigma1 = (int)gaussSigma1_nud.Value,
                GaussSigma2 = (int)gaussSigma2_nud.Value
            };

            set
            {
                minLeft_nud.Value = value.MinLeft;
                minRight_nud.Value = value.MinRight;
                minTop_nud.Value = value.MinTop;
                minBottom_nud.Value = value.MinBottom;
                gaussSigma1_nud.Value = value.GaussSigma1;
                gaussSigma2_nud.Value = value.GaussSigma2;
            }
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

            test_btn.MouseDown += (sender, args) => TestStart?.Invoke(this, EventArgs.Empty);
            test_btn.MouseUp += (sender, args) => TestEnd?.Invoke(this, EventArgs.Empty);
            apply_btn.Click += (sender, args) => Apply?.Invoke(this, EventArgs.Empty);
        }
    }
}
