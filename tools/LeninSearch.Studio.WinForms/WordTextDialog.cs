using System.Windows.Forms;

namespace LeninSearch.Studio.WinForms
{
    public partial class WordTextDialog : Form
    {
        public string WordText
        {
            get => word_tb.Text;
            set => word_tb.Text = value;
        }
        public WordTextDialog()
        {
            InitializeComponent();

            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            word_tb.KeyDown += (sender, args) =>
            {
                if (args.KeyCode != Keys.Enter) return;

                DialogResult = DialogResult.OK;
                Close();
            };

            Shown += (sender, args) => word_tb.Focus();
        }
    }
}
