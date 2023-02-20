using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.WinForms.Controls.BlockDetails;

namespace BookProject.WinForms.Controls
{
    public partial class BlockDetailsControl : UserControl
    {
        private BookViewModel _bookVm;
        private readonly CommentLinkBlockDetailsControl _commentLinkBlockDetailsControl;
        private readonly TitleBlockDetailsControl _titleBlockDetailsControl;

        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;

            _bookVm.SelectedBlockChanged += SelectedBlockChanged;
        }

        private void SelectedBlockChanged(object sender, Block e)
        {
            Controls.Clear();

            if (e is CommentLinkBlock commentLinkBlock)
            {
                _commentLinkBlockDetailsControl.SetBlock(_bookVm, commentLinkBlock);
                Controls.Add(_commentLinkBlockDetailsControl);
                _commentLinkBlockDetailsControl.Dock = DockStyle.Fill;
            }

            if (e is TitleBlock titleBlock)
            {
                _titleBlockDetailsControl.SetBlock(_bookVm, titleBlock);
                Controls.Add(_titleBlockDetailsControl);
                _titleBlockDetailsControl.Dock = DockStyle.Fill;
            }
        }

        public BlockDetailsControl()
        {
            InitializeComponent();
            _commentLinkBlockDetailsControl = new CommentLinkBlockDetailsControl();
            _titleBlockDetailsControl = new TitleBlockDetailsControl();
        }
    }
}
