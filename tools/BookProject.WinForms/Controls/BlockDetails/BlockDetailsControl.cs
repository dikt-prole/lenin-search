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
        private readonly LineBlockDetailsControl _lineBlockDetailsControl;

        public void Bind(BookViewModel bookVm)
        {
            _bookVm = bookVm;

            _bookVm.SelectedBlockChanged += BookVmSelectedBlockChanged;
        }

        private void BookVmSelectedBlockChanged(object sender, Block e)
        {
            Controls.Clear();

            if (e is CommentLinkBlock commentLinkBlock)
            {
                _commentLinkBlockDetailsControl.Bind(_bookVm, commentLinkBlock);
                Controls.Add(_commentLinkBlockDetailsControl);
                _commentLinkBlockDetailsControl.Dock = DockStyle.Fill;
            }

            if (e is TitleBlock titleBlock)
            {
                _titleBlockDetailsControl.Bind(_bookVm, titleBlock);
                Controls.Add(_titleBlockDetailsControl);
                _titleBlockDetailsControl.Dock = DockStyle.Fill;
            }

            if (e is Line line)
            {
                _lineBlockDetailsControl.Bind(_bookVm, line);
                Controls.Add(_lineBlockDetailsControl);
                _lineBlockDetailsControl.Dock = DockStyle.Fill;
            }
        }

        public BlockDetailsControl()
        {
            InitializeComponent();
            _commentLinkBlockDetailsControl = new CommentLinkBlockDetailsControl();
            _titleBlockDetailsControl = new TitleBlockDetailsControl();
            _lineBlockDetailsControl = new LineBlockDetailsControl();
        }
    }
}
