using System.Linq;
using System.Windows.Forms;

namespace LeninSearch.Studio.WinForms.Controls
{
    public partial class TitleTokenControl : UserControl
    {
        public string Text
        {
            get => token_tb.Text;
            set
            {
                var textSize = TextRenderer.MeasureText(value, token_tb.Font);
                Width = textSize.Width < 40 ? 40 : textSize.Width;
                token_tb.Text = value;
            }
        }

        public TitleTokenControl()
        {
            InitializeComponent();

            up_btn.Click += (sender, args) =>
            {
                var letters = token_tb.Text.ToList();
                for (var i = 0; i < letters.Count; i++)
                {
                    if (char.IsLetter(letters[i]))
                    {
                        letters[i] = char.ToUpper(letters[i]);
                        Text = new string(letters.ToArray());
                        return;
                    }
                }
            };

            remove_btn.Click += (sender, args) =>
            {
                Parent?.Controls.Remove(this);
                Dispose();
            };
        }
    }
}
