using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using LeninSearch.Standard.Core.Corpus.Json;
using Newtonsoft.Json;

namespace LeninSearch.JsonHeadings
{
    public partial class JsonHeadingsForm : Form
    {
        public JsonHeadingsForm()
        {
            InitializeComponent();
            file_tb.DoubleClick += (sender, args) =>
            {
                var dialog = new OpenFileDialog {Filter = "JSON|*.json"};
                if (dialog.ShowDialog() != DialogResult.OK) return;
                file_tb.Text = dialog.FileName;
            };
            headings_flp.SizeChanged += (sender, args) =>
            {
                var controls = headings_flp.Controls.OfType<JsonHeadingControl>().ToList();
                foreach (var jhc in controls)
                {
                    jhc.Width = headings_flp.Width - 26;
                }
            };
            load_btn.Click += LoadBtnClick;
            save_btn.Click += SaveBtnClick;
        }

        private void SaveBtnClick(object? sender, EventArgs e)
        {
            var controls = headings_flp.Controls.OfType<JsonHeadingControl>().Where(c => c.IsEnabled).ToList();
            var headings = controls.Select(c => c.GetHeading()).OrderBy(h => h.Index).ToList();

            var json = File.ReadAllText(file_tb.Text);
            var fd = JsonConvert.DeserializeObject<JsonFileData>(json);
            fd.Headings = headings;

            json = JsonConvert.SerializeObject(fd, Formatting.Indented);
            File.WriteAllText(file_tb.Text, json);
        }

        private void LoadBtnClick(object? sender, EventArgs e)
        {
            ClearHeadings();

            var json = File.ReadAllText(file_tb.Text);
            var fd = JsonConvert.DeserializeObject<JsonFileData>(json);

            var headings = fd.Headings.OrderBy(h => h.Index).ToList();
            foreach (var heading in headings)
            {
                var control = new JsonHeadingControl();
                control.SetHeading(heading);
                headings_flp.Controls.Add(control);
                control.Width = headings_flp.Width - 26;
            }
        }

        private void ClearHeadings()
        {
            var controls = headings_flp.Controls.OfType<JsonHeadingControl>().ToList();
            foreach (var jhc in controls)
            {
                headings_flp.Controls.Remove(jhc);
                jhc.Dispose();
            }
        }
    }
}
