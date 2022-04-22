using System.Reflection;
using System.Windows.Forms;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
{
    public partial class LineFeaturesDialog : Form
    {
        public LineFeaturesDialog()
        {
            InitializeComponent();

            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public void SetFeatures(OcrLineFeatures features)
        {
            features_lb.Items.Clear();

            if (features == null) return;

            var props = typeof(OcrLineFeatures).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                features_lb.Items.Add($"{prop.Name} = {(double) prop.GetValue(features):F2}");
            }
        }
    }
}
