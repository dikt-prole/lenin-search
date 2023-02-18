using System;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class CommentLinkBlockDetailsControl : UserControl
    {
        private CommentLinkBlock _commentLinkBlock;
        private BookViewModel _bookVm;

        public CommentLinkBlockDetailsControl()
        {
            InitializeComponent();
            commentText_tb.KeyUp += (sender, args) =>
            {
                if (_commentLinkBlock != null)
                {
                    _bookVm.ModifyBlock(this, _commentLinkBlock, b =>
                    {
                        b.CommentText = commentText_tb.Text;
                    });
                }
            };
        }

        public void SetBlock(BookViewModel bookVm, CommentLinkBlock commentLinkBlock)
        {
            _bookVm = bookVm;
            _commentLinkBlock = commentLinkBlock;
            commentText_tb.Text = _commentLinkBlock.CommentText;
        }
    }
}
