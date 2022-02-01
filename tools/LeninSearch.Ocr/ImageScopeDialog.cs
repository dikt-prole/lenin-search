using System.Windows.Forms;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
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

        public bool BlockMatch(OcrFeaturedBlock block)
        {
            return MinImageIndex <= block.ImageIndex && block.ImageIndex <= MaxImageIndex;
        }

        public bool ImageMatch(int imageIndex)
        {
            return MinImageIndex <= imageIndex && imageIndex <= MaxImageIndex;
        }
    }
}
