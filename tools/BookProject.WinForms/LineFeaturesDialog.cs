using System.Reflection;
using System.Windows.Forms;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;

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

        public void SetFeatures(OldBookProjectFeatures features)
        {
            features_lb.Items.Clear();

            if (features == null) return;

            var props = typeof(OldBookProjectFeatures).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var prop in props)
            {
                features_lb.Items.Add($"{prop.Name} = {(double) prop.GetValue(features):F2}");
            }
        }
    }
}
