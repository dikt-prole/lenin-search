using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LeninSearch.Ocr
{
    public partial class TitleBlockDialog : Form
    {
        public string TitleText
        {
            get => titleText_tb.Text;
            set => titleText_tb.Text = value;
        }

        public int TitleLevel
        {
            get => (int)level_nud.Value;
            set => level_nud.Value = value;
        }

        public TitleBlockDialog()
        {
            InitializeComponent();
            level_nud.Minimum = 0;
            level_nud.Maximum = 10;
            level_nud.Value = 0;
            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
