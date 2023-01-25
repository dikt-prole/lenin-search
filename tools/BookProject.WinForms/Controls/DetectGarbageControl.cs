using System;
using System.Windows.Forms;
using BookProject.Core.Settings;

namespace BookProject.WinForms.Controls
{
    public partial class DetectGarbageControl : UserControl
    {
        public event EventHandler TestStart;

        public event EventHandler TestEnd;

        public event EventHandler Apply;

        public event EventHandler Save;
        public DetectGarbageSettings GetSettings()
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

        public void SetSettings(DetectGarbageSettings settings)
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

            test_btn.MouseDown += (sender, args) => TestStart?.Invoke(this, EventArgs.Empty);
            test_btn.MouseUp += (sender, args) => TestEnd?.Invoke(this, EventArgs.Empty);
            apply_btn.Click += (sender, args) => Apply?.Invoke(this, EventArgs.Empty);
            save_btn.Click += (sender, args) => Save?.Invoke(this, EventArgs.Empty);
        }
    }
}
