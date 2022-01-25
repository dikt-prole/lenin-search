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
            loadBlocks_btn.Click += loadBlocks_btnOnClick;
            saveLabeled_btn.Click += SaveLabeled_btnOnClick;
            saveAll_btn.Click += SaveAll_btnOnClick;
            ocrBlock_lb.SelectedIndexChanged += OcrBlock_lbOnSelectedIndexChanged;
            ocrBlock_lb.KeyDown += OcrBlock_lbOnKeyDown;

            paragraph_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Paragraph);
            title_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Title);
            comment_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Comment);
            grabage_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Garbage);
            none_panel.BackColor = BlockPalette.GetColor(null);
        }

        private void OcrBlock_lbOnKeyDown(object sender, KeyEventArgs e)
        {
            var row = ocrBlock_lb.SelectedItem as OcrBlockRow;

            if (row == null) return;

            if (e.KeyCode == Keys.P) row.Label = OcrBlockLabel.Paragraph;

            if (e.KeyCode == Keys.C) row.Label = OcrBlockLabel.Comment;

            if (e.KeyCode == Keys.T) row.Label = OcrBlockLabel.Title;

            if (e.KeyCode == Keys.G) row.Label = OcrBlockLabel.Garbage;

            if (e.KeyCode == Keys.N) row.Label = null;

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
            var color = BlockPalette.GetColor(row.Label);

            return new Pen(color, 4);
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

        private void loadBlocks_btnOnClick(object? sender, EventArgs e)
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
