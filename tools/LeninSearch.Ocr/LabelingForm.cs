using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CsvHelper;
using LeninSearch.Ocr.Labeling;
using LeninSearch.Ocr.YandexVision.OcrResponse;
using Newtonsoft.Json;

namespace LeninSearch.Ocr
{
    public partial class LabelingForm : Form
    {
        private List<OcrBlockRow> _blockRows;

        public LabelingForm()
        {
            InitializeComponent();
            load_btn.Click += Load_btnOnClick;
            saveLabeled_btn.Click += SaveLabeled_btnOnClick;
            saveAll_btn.Click += SaveAll_btnOnClick;
            ocrBlock_lb.SelectedIndexChanged += OcrBlock_lbOnSelectedIndexChanged;
            garbage_rb.CheckedChanged += LabelRbOnCheckedChanged;
            paragraph_rb.CheckedChanged += LabelRbOnCheckedChanged;
            title_rb.CheckedChanged += LabelRbOnCheckedChanged;
            comment_rb.CheckedChanged += LabelRbOnCheckedChanged;
            none_rb.CheckedChanged += LabelRbOnCheckedChanged;
            ocrBlock_lb.KeyDown += OcrBlock_lbOnKeyDown;
        }

        

        private void OcrBlock_lbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P) paragraph_rb.Checked = true;

            if (e.KeyCode == Keys.C) comment_rb.Checked = true;

            if (e.KeyCode == Keys.T) title_rb.Checked = true;

            if (e.KeyCode == Keys.G) garbage_rb.Checked = true;

            if (e.KeyCode == Keys.N) none_rb.Checked = true;
        }

        private void LabelRbOnCheckedChanged(object? sender, EventArgs e)
        {
            var row = ocrBlock_lb.SelectedItem as OcrBlockRow;

            if (row == null) return;

            if (garbage_rb.Checked) row.Label = OcrBlockLabel.Garbage;

            if (title_rb.Checked) row.Label = OcrBlockLabel.Title;

            if (paragraph_rb.Checked) row.Label = OcrBlockLabel.Paragraph;

            if (comment_rb.Checked) row.Label = OcrBlockLabel.Comment;

            if (none_rb.Checked) row.Label = null;

            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = row;
        }

        private void OcrBlock_lbOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            var row = ocrBlock_lb.SelectedItem as OcrBlockRow;
            
            if (row == null) return;

            var bookFolder = Path.GetDirectoryName(csvFile_tb.Text);

            var imageFolder = Path.Combine(bookFolder, "images");
            var jsonFolder = Path.Combine(bookFolder, "json");

            var imageFile = Directory.GetFiles(imageFolder).FirstOrDefault(f => f.Contains($"{row.FileName}."));
            if (imageFile == null)
            {
                MessageBox.Show("Image file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var jsonFile = Directory.GetFiles(jsonFolder).FirstOrDefault(f => f.Contains($"{row.FileName}."));
            if (jsonFile == null)
            {
                MessageBox.Show("Json file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var image = new Bitmap(Image.FromFile(imageFile));
            var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));
            var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];
            var block = page.Blocks[row.BlockIndex];
            var box = block.BoundingBox;
            using (var g = Graphics.FromImage(image))
            {
                var pen = GetPen(row);

                g.DrawLine(pen, box.TopLeft.Point(), box.BottomLeft.Point());
                g.DrawLine(pen, box.BottomLeft.Point(), box.BottomRight.Point());
                g.DrawLine(pen, box.BottomRight.Point(), box.TopRight.Point());
                g.DrawLine(pen, box.TopRight.Point(), box.TopLeft.Point());

                //DrawFeatures(g, box, row);
            }

            pictureBox1.Image = image;

            if (row.Label == null) none_rb.Checked = true;

            if (row.Label == OcrBlockLabel.Comment) comment_rb.Checked = true;

            if (row.Label == OcrBlockLabel.Garbage) garbage_rb.Checked = true;

            if (row.Label == OcrBlockLabel.Paragraph) paragraph_rb.Checked = true;

            if (row.Label == OcrBlockLabel.Title) title_rb.Checked = true;
        }

        private void DrawFeatures(Graphics g, BoundingBox box, OcrBlockRow row)
        {
            var font = new Font(Font.FontFamily, 12, FontStyle.Bold);

            var leftIndentX = box.TopLeft.Point().X;
            var leftIndentY = (box.BottomLeft.Point().Y + box.TopLeft.Point().Y) / 2;
            g.DrawString(row.LeftIndent.ToString(), font, Brushes.Blue, leftIndentX, leftIndentY);

            var rightIndentX = box.TopRight.Point().X - 50;
            var rightIndentY = leftIndentY;
            g.DrawString(row.RightIndent.ToString(), font, Brushes.Blue, rightIndentX, rightIndentY);

            var topIndentX = (box.TopRight.Point().X + box.TopLeft.Point().X) / 2;
            var topIndentY = box.TopLeft.Point().Y - 30;
            g.DrawString(row.TopIndent.ToString(), font, Brushes.Blue, topIndentX, topIndentY);

            var bottomIndentX = topIndentX;
            var bottomIndentY = box.BottomLeft.Point().Y + 10;
            g.DrawString(row.BottomIndent.ToString(), font, Brushes.Blue, bottomIndentX, bottomIndentY);

            var pixelsPerSymbolX = (box.TopRight.Point().X + box.TopLeft.Point().X) / 2;
            var pixelsPerSymbolY = (box.BottomLeft.Point().Y + box.TopLeft.Point().Y) / 2 - 20;
            g.DrawString(row.PixelsPerSymbol.ToString("F2"), font, Brushes.DeepPink, pixelsPerSymbolX, pixelsPerSymbolY);
        }


        private Pen GetPen(OcrBlockRow row)
        {
            if (row.Label == null) return new Pen(Color.Gray, 4);

            if (row.Label == OcrBlockLabel.Garbage) return new Pen(Color.Brown, 4);

            if (row.Label == OcrBlockLabel.Paragraph) return new Pen(Color.Green, 4);

            if (row.Label == OcrBlockLabel.Comment) return new Pen(Color.DodgerBlue, 4);

            if (row.Label == OcrBlockLabel.Title) return new Pen(Color.Red, 4);

            return null;
        }

        private void SaveLabeled_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save labeled rows",
                Filter = "CSV|*.csv"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var labeledRows = new List<OcrBlockRow>();
            foreach (OcrBlockRow row in ocrBlock_lb.Items)
            {
                if (row.Label != null) labeledRows.Add(row);
            }

            if (labeledRows.Count == 0)
            {
                MessageBox.Show("Rows are not labeled", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var csv = new CsvWriter(new StreamWriter(dialog.FileName), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(labeledRows);
            }

            MessageBox.Show($"Saved {labeledRows.Count} to '{dialog.FileName}'", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveAll_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save all rows",
                Filter = "CSV|*.csv"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var rows = new List<OcrBlockRow>();
            foreach (OcrBlockRow row in ocrBlock_lb.Items) rows.Add(row);

            using (var csv = new CsvWriter(new StreamWriter(dialog.FileName), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(rows);
            }

            MessageBox.Show($"Saved {rows.Count} to '{dialog.FileName}'", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Load_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load ocr block CSV",
                Filter = "CSV|*.csv"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            csvFile_tb.Text = dialog.FileName;

            using (var csv = new CsvReader(new StreamReader(dialog.FileName), CultureInfo.InvariantCulture))
            {
                _blockRows = csv.GetRecords<OcrBlockRow>().ToList();
            }

            ocrBlock_lb.Items.Clear();
            foreach (var blockRow in _blockRows)
            {
                ocrBlock_lb.Items.Add(blockRow);
            }
        }
    }
}
