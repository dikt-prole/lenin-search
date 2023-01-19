using System.Windows.Forms;
using LeninSearch.Studio.WinForms.Model;

namespace LeninSearch.Studio.WinForms
{
    public partial class ImageScopeDialog : Form
    {
        public int MaxImageIndex => (int)maxImageIndex_nud.Value;
        public int MinImageIndex => (int)minImageIndex_nud.Value;

        public ImageScopeDialog()
        {
            InitializeComponent();

            maxImageIndex_nud.Minimum = 1;
            maxImageIndex_nud.Maximum = 10000;
            maxImageIndex_nud.Value = 1000;

            minImageIndex_nud.Minimum = 0;
            minImageIndex_nud.Maximum = 10000;
            minImageIndex_nud.Value = 0;

            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public bool PageMatch(OcrPage page)
        {
            return MinImageIndex <= page.ImageIndex && page.ImageIndex <= MaxImageIndex;
        }

        public bool ImageMatch(int imageIndex)
        {
            return MinImageIndex <= imageIndex && imageIndex <= MaxImageIndex;
        }
    }
}
