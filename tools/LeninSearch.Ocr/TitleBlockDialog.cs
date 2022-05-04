using System;
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
            Shown += (sender, args) => titleText_tb.Focus();
        }

        private void ToUpperCaseClick(object sender, EventArgs e)
        {
            var selectionStart = titleText_tb.SelectionStart;
            var selectionLength = titleText_tb.SelectionLength;

            titleText_tb.Text = titleText_tb.Text.Substring(0, selectionStart) +
                                titleText_tb.Text.Substring(selectionStart, selectionLength).ToUpper() +
                                titleText_tb.Text.Substring(selectionStart + selectionLength);
        }
    }
}
