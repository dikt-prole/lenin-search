using System;
using System.Linq;
using System.Windows.Forms;
using BookProject.WinForms.Controls;

namespace BookProject.WinForms
{
    public partial class TitleBlockDialog : Form
    {
        public string TitleText
        {
            get => string.Join(' ', tokens_flp.Controls.OfType<TitleTokenControl>().Select(c => c.Text));
            set
            {
                tokens_flp.Controls.Clear();
                var spaceSplit = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in spaceSplit)
                {
                    var tokenControl = new TitleTokenControl();
                    tokenControl.Text = s;
                    tokens_flp.Controls.Add(tokenControl);
                }
            }
        }

        public int TitleLevel
        {
            get => (int)level_nud.Value;
            set => level_nud.Value = value;
        }

        public TitleBlockDialog()
        {
            InitializeComponent();
            level_nud.Minimum = 0;
            level_nud.Maximum = 10;
            level_nud.Value = 0;
            ok_btn.Click += (sender, args) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };
        }
    }
}
