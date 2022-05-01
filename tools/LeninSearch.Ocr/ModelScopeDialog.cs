using System.Windows.Forms;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
{
    public partial class ModelScopeDialog : Form
    {
        public int TakeBefore
        {
            get => (int)takeBefore_nud.Value;
            set => takeBefore_nud.Value = value;
        }

        public int ImageIndex
        {
            get => (int)imageIndex_nud.Value;
            set => imageIndex_nud.Value = value;
        }
        public int TakeAfter
        {
            get => (int)takeAfter_nud.Value;
            set => takeAfter_nud.Value = value;
        }

        public ModelScopeDialog()
        {
            InitializeComponent();

            imageIndex_nud.Minimum = 0;
            imageIndex_nud.Maximum = 10000;
            imageIndex_nud.Value = 0;
            imageIndex_nud.ReadOnly = true;
            imageIndex_nud.Enabled = false;

            takeBefore_nud.Minimum = 0;
            takeBefore_nud.Maximum = 10000;
            takeBefore_nud.Value = 0;

            takeAfter_nud.Minimum = 0;
            takeAfter_nud.Maximum = 10000;
            takeAfter_nud.Value = 0;


            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public bool BeforePageMatch(OcrPage page)
        {
            return ImageIndex - TakeBefore <= page.ImageIndex && page.ImageIndex <= ImageIndex;
        }

        public bool AfterPageMatch(OcrPage page)
        {
            return ImageIndex < page.ImageIndex && page.ImageIndex <= ImageIndex + TakeAfter;
        }
    }
}
