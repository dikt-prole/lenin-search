using System;
using System.Windows.Forms;
using LenLib.Standard.Core.Corpus.Json;

namespace LenLib.JsonHeadings
{
    public partial class JsonHeadingControl : UserControl
    {
        public event Action<JsonHeadingControl> InsertClick;
        public bool IsEnabled => enabled_chb.Checked;
        public JsonHeadingControl()
        {
            InitializeComponent();

            index_nud.Minimum = 0;
            index_nud.Maximum = ushort.MaxValue;

            level_nud.Minimum = 0;
            level_nud.Maximum = 10;

            level_nud.ValueChanged +=
                (sender, args) => text_tb.Margin = new Padding(3 + 20 * (int) level_nud.Value, 3, 3, 3);
            insert_btn.Click += (sender, args) => InsertClick?.Invoke(this);
        }

        public JsonHeading GetHeading()
        {
            return new JsonHeading
            {
                Index = (ushort) index_nud.Value,
                Level = (byte) level_nud.Value,
                Text = text_tb.Text
            };
        }

        public void SetHeading(JsonHeading heading)
        {
            index_nud.Value = heading.Index;
            level_nud.Value = heading.Level;
            text_tb.Text = heading.Text;
            enabled_chb.Checked = true;
        }
    }
}
