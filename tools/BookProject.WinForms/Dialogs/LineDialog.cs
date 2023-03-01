using System;
using System.Drawing;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;

namespace BookProject.WinForms.Dialogs
{
    public partial class LineDialog : Form
    {
        public LineDialog()
        {
            InitializeComponent();
            Icon = new Icon("book_project_icon.ico");
            ShowInTaskbar = false;
            ok_btn.Click += OkBtnOnCLick;
            replace_tb.KeyDown += ReplaceTbOnKeyDown;
            replace_btn.Click += ReplaceBtnOnClick;
            Shown += OnShown;
        }

        private void OnShown(object sender, EventArgs e)
        {
            replace_tb.SelectAll();
            replace_tb.Focus();
        }

        private void ReplaceBtnOnClick(object sender, EventArgs e)
        {
            replace_tb.Text = original_tb.Text;
            replace_rb.Checked = true;
            replace_tb.SelectAll();
            replace_tb.Focus();
        }

        private void ReplaceTbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                DialogResult = DialogResult.OK;
                Close();
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void OkBtnOnCLick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public LineDialog Init(Line line)
        {
            first_chb.Checked = line.Type == LineType.First;
            replace_rb.Checked = line.Replace;
            original_rb.Checked = !line.Replace;
            original_tb.Text = line.GetOriginalTextPreview();
            replace_tb.Text = line.ReplaceText;
            return this;
        }

        public void Apply(Line line)
        {
            line.Type = first_chb.Checked ? LineType.First : LineType.Normal;
            line.Replace = replace_rb.Checked;
            line.ReplaceText = replace_rb.Checked 
                ? replace_tb.Text
                : null;
        }
    }
}
