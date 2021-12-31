using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using Newtonsoft.Json;

namespace LeninSearch.YtEditor
{
    public partial class YtEditorForm : Form
    {
        public YtEditorForm()
        {
            InitializeComponent();
            file_tb.Text = @"D:\Repo\lenin-search\corpus\json\KLGD\yt0j8q2xiULRM.json";
            paragraphs_flp.SizeChanged += (o, e) =>
            {
                foreach (var c in paragraphs_flp.Controls.OfType<ParagraphControl>())
                {
                    c.Width = paragraphs_flp.Width - 26;
                }
            };
        }

        private void load_btn_Click(object sender, EventArgs e)
        {
            var fileData = JsonConvert.DeserializeObject<JsonFileData>(File.ReadAllText(file_tb.Text));

            ClearParagaphsFlp();

            foreach (var paragraph in fileData.Pars)
            {
                var control = new ParagraphControl
                {
                    ParagraphText = paragraph.Text,
                    OffsetSeconds = paragraph.OffsetSeconds,
                    VideoId = Path.GetFileName(file_tb.Text).Substring(2).Replace(".json", "")
                };

                paragraphs_flp.Controls.Add(control);
            }
        }

        private void ClearParagaphsFlp()
        {
            var controls = paragraphs_flp.Controls.OfType<ParagraphControl>().ToList();

            foreach (var control in controls)
            {
                paragraphs_flp.Controls.Remove(control);

                control.Dispose();
            }
        }

        private void save_btn_Click(object sender, EventArgs e)
        {
            var controls = paragraphs_flp.Controls.OfType<ParagraphControl>().ToList();

            var paragraphs = controls.Select(c => new JsonParagraph
            {
                Text = c.ParagraphText,
                OffsetSeconds = c.OffsetSeconds,
                ParagraphType = JsonParagraphType.Youtube
            }).ToList();

            var fileData = new JsonFileData {Pars = paragraphs};

            File.WriteAllText(file_tb.Text, JsonConvert.SerializeObject(fileData, Formatting.Indented));

            MessageBox.Show($"Saved to '{file_tb.Text}'", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
