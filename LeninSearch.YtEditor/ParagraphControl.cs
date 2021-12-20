using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LeninSearch.YtEditor
{
    public partial class ParagraphControl : UserControl
    {
        public ushort Index { get; set; }
        public ushort OffsetSeconds { get; set; }
        public string VideoId { get; set; }
        public string ParagraphText
        {
            get => paragraph_tb.Text;
            set => paragraph_tb.Text = value;
        }

        public ParagraphControl()
        {
            InitializeComponent();
            Height = 50;
        }

        private void yt_btn_Click(object sender, EventArgs e)
        {
            var link = $"https://youtu.be/{VideoId}?t={OffsetSeconds}";
            Process.Start("cmd", "/C start" + " " + link);
        }
    }
}
