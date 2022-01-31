using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LeninSearch.Ocr
{
    public partial class TrainModelDialog : Form
    {
        public int MaxImageIndex => (int)maxImageIndex_nud.Value;

        public TrainModelDialog()
        {
            InitializeComponent();
            maxImageIndex_nud.Minimum = 1;
            maxImageIndex_nud.Maximum = 10000;
            maxImageIndex_nud.Value = 1000;
            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
