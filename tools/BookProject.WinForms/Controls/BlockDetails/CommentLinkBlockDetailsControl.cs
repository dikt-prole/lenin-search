using System;
using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class CommentLinkBlockDetailsControl : UserControl
    {
        public EventHandler<CommentLinkBlock> BlockChanged;

        private CommentLinkBlock _commentLinkBlock;
        public CommentLinkBlockDetailsControl()
        {
            InitializeComponent();
            commentText_tb.KeyUp += (sender, args) =>
            {
                if (_commentLinkBlock != null)
                {
                    _commentLinkBlock.CommentText = commentText_tb.Text;
                    BlockChanged?.Invoke(this, _commentLinkBlock);
                }
            };
        }

        public void SetBlock(CommentLinkBlock commentLinkBlock)
        {
            _commentLinkBlock = commentLinkBlock;
            commentText_tb.Text = _commentLinkBlock.CommentText;
        }
    }
}
