using System;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using Keys = System.Windows.Forms.Keys;

namespace BookProject.WinForms.Dialogs
{
    public partial class CommentLinkDialog : Form
    {
        public CommentLinkDialog()
        {
            InitializeComponent();
            Shown += OnShown;
            commentText_tb.KeyDown += CommentTextTbOnKeyDown;
            ok_btn.Click += OkBtnOnClick;
        }

        private void OkBtnOnClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CommentTextTbOnKeyDown(object sender, KeyEventArgs e)
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

        private void OnShown(object sender, EventArgs e)
        {
            commentText_tb.SelectAll();
            commentText_tb.Focus();
        }

        public CommentLinkDialog Init(CommentLinkBlock commentLinkBlock)
        {
            commentText_tb.Text = commentLinkBlock.CommentText;
            return this;
        }

        public void Apply(CommentLinkBlock commentLinkBlock)
        {
            commentLinkBlock.CommentText = commentText_tb.Text;
        }
    }
}
