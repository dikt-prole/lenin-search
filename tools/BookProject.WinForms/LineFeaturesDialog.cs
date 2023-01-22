using System.Reflection;
using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms
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

        public void SetFeatures(BookProjectFeatures features)
        {
            features_lb.Items.Clear();

            if (features == null) return;

            var props = typeof(BookProjectFeatures).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                features_lb.Items.Add($"{prop.Name} = {(double) prop.GetValue(features):F2}");
            }
        }
    }
}
