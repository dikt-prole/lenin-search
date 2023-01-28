using System;
using System.Windows.Forms;
using BookProject.Core.Settings;

namespace BookProject.WinForms.Controls
{
    public partial class DetectCommentLinkNumberControl : UserControl
    {
        public event EventHandler TestStart;

        public event EventHandler TestEnd;

        public event EventHandler Detect;

        public event EventHandler Save;
        public DetectCommentLinkNumberSettings GetSettings()
        {
            return new DetectCommentLinkNumberSettings
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
                AllowedSymbols = allowedSymbols_tb.Text.ToCharArray()
            };
        }

        public void SetSettings(DetectCommentLinkNumberSettings settings)
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
            allowedSymbols_tb.Text = settings.AllowedSymbols == null 
                ? "" 
                : new string(settings.AllowedSymbols);
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

            allowedSymbols_tb.Text = "*";

            test_btn.MouseDown += (sender, args) => TestStart?.Invoke(this, EventArgs.Empty);
            test_btn.MouseUp += (sender, args) => TestEnd?.Invoke(this, EventArgs.Empty);
            detect_btn.Click += (sender, args) => Detect?.Invoke(this, EventArgs.Empty);
            save_btn.Click += (sender, args) => Save?.Invoke(this, EventArgs.Empty);
        }
    }
}
