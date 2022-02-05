using System.Windows.Forms;

namespace LeninSearch.Ocr
{
    public partial class AddWordDialog : Form
    {
        public string WordText => word_tb.Text;
        public AddWordDialog()
        {
            InitializeComponent();

            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            Shown += (sender, args) => word_tb.Focus();
        }
    }
}
