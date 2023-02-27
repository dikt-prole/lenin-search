using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class LineBlockDetailsControl : UserControl
    {
        private BookViewModel _bookVm;
        private Line _line;

        public LineBlockDetailsControl()
        {
            InitializeComponent();
            original_rb.MouseUp += (sender, args) => Apply();
            replace_rb.MouseUp += (sender, args) => Apply();
            original_tb.KeyUp += (sender, args) => Apply();
            replace_tb.KeyUp += (sender, args) => Apply();
            first_chb.MouseUp += (sender, args) => Apply();
        }

        public void Bind(BookViewModel bookVm, Line line)
        {
            _bookVm = bookVm;
            _line = line;
            first_chb.Checked = _line.Type == LineType.First;
            replace_rb.Checked = line.Replace;
            original_rb.Checked = !line.Replace;
            original_tb.Text = line.GetOriginalTextPreview();
            replace_tb.Text = line.ReplaceText;
        }

        private void Apply()
        {
            if (_line == null) return;

            _bookVm.ModifyBlock(this, _line, l =>
            {
                l.Type = first_chb.Checked ? LineType.First : LineType.Normal;
                l.Replace = replace_rb.Checked;
                l.ReplaceText = replace_tb.Text;
            });
        }
    }
}
