using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class CommentLinkBlockDetailsControl : UserControl
    {
        private CommentLinkBlock _commentLinkBlock;
        public CommentLinkBlockDetailsControl()
        {
            InitializeComponent();
            useText_tb.KeyUp += (sender, args) =>
            {
                if (_commentLinkBlock != null)
                {
                    _commentLinkBlock.UseText = useText_tb.Text;
                }
            };
        }

        public void SetBlock(CommentLinkBlock commentLinkBlock)
        {
            _commentLinkBlock = commentLinkBlock;
            useText_tb.Text = _commentLinkBlock.UseText;
        }
    }
}
