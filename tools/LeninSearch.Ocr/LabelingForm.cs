using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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

        private bool _loadPages;

        private Point? _selectionStartPoint;

        private List<ImageBlock> _imageBlocks;

        private Action<Point> _moveImageBlockDragPoint;

        public LabelingForm()
        {
            InitializeComponent();


            loadBlocks_mi.Click += LoadBlocks_btnOnClick;
            loadPages_mi.Click += LoadPages_btnOnClick;

            saveLabeledBlocks_mi.Click += SaveLabeled_btnOnClick;
            saveAllBlocks_mi.Click += SaveAll_btnOnClick;
            ocrBlock_lb.SelectedIndexChanged += OcrBlock_lbOnSelectedIndexChanged;
            ocrBlock_lb.KeyDown += OcrBlock_lbOnKeyDown;

            paragraph_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Paragraph);
            title_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Title);
            comment_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Comment);
            grabage_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Garbage);
            image_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Image);
            none_panel.BackColor = BlockPalette.GetColor(null);

            pictureBox1.Paint += PictureBox1OnPaint;
            pictureBox1.MouseDown += PictureBox1OnMouseDown;
            pictureBox1.MouseUp += PictureBox1OnMouseUp;
            pictureBox1.MouseMove += PictureBox1OnMouseMove;

            autoAddImageBlocks_mi.Click += AutoAddImageBlocks_btnOnClick;
            saveImageBlocks_mi.Click += SaveImageBlocks_btnOnClick;
            loadImageBlocks_mi.Click += LoadImageBlocks_btnOnClick;
        }

        private void LoadImageBlocks_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON|*.json",
                Title = "Load image blocks"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _imageBlocks = JsonConvert.DeserializeObject<List<ImageBlock>>(File.ReadAllText(dialog.FileName));

            MessageBox.Show($"Image blocks loaded", "Image blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveImageBlocks_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "JSON|*.json",
                Title = "Save image blocks"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            if (_imageBlocks == null) return;

            var imageBlocksJson = JsonConvert.SerializeObject(_imageBlocks);
            File.WriteAllText(dialog.FileName, imageBlocksJson);

            MessageBox.Show($"Image blocks saved to '{dialog.FileName}'", "Image blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AutoAddImageBlocks_btnOnClick(object sender, EventArgs e)
        {
            // 1. get image row groups
            var fileBlockGroups = new Dictionary<string, List<List<OcrBlockRow>>>();
            var fileNames = _blockRows.OrderBy(r => r.ImageIndex).Select(r => r.FileName).Distinct().ToList();
            foreach (var fileName in fileNames)
            {
                var blockGroups = new List<List<OcrBlockRow>>();
                var blockGroup = new List<OcrBlockRow>();
                var rows = _blockRows.Where(r => r.FileName == fileName).OrderBy(r => r.BlockIndex).ToList();

                foreach (var row in rows)
                {
                    if (row.Label != OcrBlockLabel.Image && blockGroup.Any())
                    {
                        blockGroups.Add(blockGroup);
                        blockGroup = new List<OcrBlockRow>();
                    }
                    else if (row.Label == OcrBlockLabel.Image)
                    {
                        blockGroup.Add(row);
                    }
                }

                if (blockGroup.Any())
                {
                    blockGroups.Add(blockGroup);
                }

                if (blockGroups.Any())
                {
                    fileBlockGroups.Add(fileName, blockGroups);
                }
            }

            // 2. generate image blocks
            var bookFolder = Path.GetDirectoryName(csvFile_tb.Text);
            var jsonFolder = Path.Combine(bookFolder, "json");
            var imageFolder = Path.Combine(bookFolder, "images");
            _imageBlocks = new List<ImageBlock>();
            foreach (var fileName in fileBlockGroups.Keys)
            {
                var jsonFile = Path.Combine(jsonFolder, fileName + ".json");
                var imageFile = Directory.GetFiles(imageFolder).First(f => Path.GetFileNameWithoutExtension(f) == fileName);
                using var image = Image.FromFile(imageFile);

                var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));
                var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];
                foreach (var blockGroup in fileBlockGroups[fileName])
                {
                    var topY = int.MaxValue;
                    var leftX = int.MaxValue;
                    var bottomY = 0;
                    var rightX = 0;

                    foreach (var row in blockGroup)
                    {
                        var block = page.Blocks[row.BlockIndex];

                        var topLeft = block.BoundingBox.TopLeft.Point();
                        if (topLeft.X < leftX) leftX = topLeft.X;
                        if (topLeft.Y < topY) topY = topLeft.Y;

                        var bottomRight = block.BoundingBox.BottomRight.Point();
                        if (bottomRight.X > rightX) rightX = bottomRight.X;
                        if (bottomRight.Y > bottomY) bottomY = bottomRight.Y;
                    }

                    var defaultLeft = 40;
                    if (leftX > defaultLeft) leftX = defaultLeft;

                    var defaultRight = image.Width - defaultLeft;
                    if (rightX < defaultRight) rightX = defaultRight;

                    var imageBlock = new ImageBlock
                    {
                        FileName = fileName,
                        TopLeftX = leftX,
                        TopLeftY = topY - 10,
                        BottomRightX = rightX,
                        BottomRightY = bottomY + 10
                    };

                    _imageBlocks.Add(imageBlock);
                }
            }

            MessageBox.Show("Image blocks were auto generated", "Image blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);
            pictureBox1.Refresh();
        }

        private void PictureBox1OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_loadPages && pictureBox1.Image != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    _selectionStartPoint = e.Location;
                }

                if (e.Button == MouseButtons.Left && ocrBlock_lb.SelectedItem is string fileName)
                {
                    _moveImageBlockDragPoint = null;
                    var fileImageBlocks = _imageBlocks?.Where(ib => ib.FileName == fileName).ToList() ?? new List<ImageBlock>();
                    foreach (var ib in fileImageBlocks)
                    {
                        var originalPoint = pictureBox1.ToOriginalPoint(e.Location);
                        if (ib.TopLeftRectangle.Contains(originalPoint))
                        {
                            _moveImageBlockDragPoint = p =>
                            {
                                if (ModifierKeys != Keys.Shift)
                                {
                                    ib.TopLeftX = p.X;
                                }
                                ib.TopLeftY = p.Y; 
                                pictureBox1.Refresh();
                            };
                            break;
                        }

                        if (ib.BottomRightRectangle.Contains(originalPoint))
                        {
                            _moveImageBlockDragPoint = p =>
                            {
                                if (ModifierKeys != Keys.Shift)
                                {
                                    ib.BottomRightX = p.X;
                                }
                                ib.BottomRightY = p.Y; 
                                pictureBox1.Refresh();
                            };
                            break;
                        }
                    }
                }
            }
        }

        private void PictureBox1OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_loadPages)
            {
                if (e.Button == MouseButtons.Right && _selectionStartPoint != null)
                {
                    var xs = new List<int> { _selectionStartPoint.Value.X, e.Location.X }.OrderBy(i => i).ToList();
                    var ys = new List<int> { _selectionStartPoint.Value.Y, e.Location.Y }.OrderBy(i => i).ToList();
                    var rect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                    var menu = new ContextMenuStrip();
                    menu.Items.Add("Image", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Image));
                    menu.Items.Add("Paragraph", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Paragraph));
                    menu.Items.Add("Comment", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Comment));
                    menu.Items.Add("Title", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Title));
                    menu.Items.Add("Garbage", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Garbage));
                    menu.Items.Add("None", null, (o, a) => SetLabelForIntersectingBlocks(rect, null));
                    menu.Items.Add("Add Image Block", null, (o, a) => AddImageBlock(rect));
                    menu.Items.Add("Remove Image Block", null, (o, a) => RemoveImageBlock(rect));
                    menu.Show(pictureBox1, e.Location);
                    _selectionStartPoint = null;
                }

                if (e.Button == MouseButtons.Left)
                {
                    _moveImageBlockDragPoint = null;
                }
            }
        }

        private void PictureBox1OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _selectionStartPoint != null)
            {
                pictureBox1.Refresh();
            }

            if (e.Button == MouseButtons.Left)
            {
                var originalPoint = pictureBox1.ToOriginalPoint(e.Location);
                _moveImageBlockDragPoint?.Invoke(originalPoint);
            }
        }

        private void PictureBox1OnPaint(object sender, PaintEventArgs e)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;
            if (fileName == null) return;

            if (_selectionStartPoint != null)
            {
                var currentPoint = pictureBox1.PointToClient(Cursor.Position);
                var xs = new List<int> { _selectionStartPoint.Value.X, currentPoint.X }.OrderBy(i => i).ToList();
                var ys = new List<int> { _selectionStartPoint.Value.Y, currentPoint.Y }.OrderBy(i => i).ToList();
                var rect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }

            var fileImageBlocks = _imageBlocks?.Where(b => b.FileName == fileName).ToList() ?? new List<ImageBlock>();
            foreach (var ib in fileImageBlocks)
            {
                using var ibPen = new Pen(BlockPalette.ImageBlockColor, 2);
                e.Graphics.DrawRectangle(ibPen, pictureBox1.ToPictureBoxRectangle(ib.Rectangle));

                using var ibBrush = new SolidBrush(BlockPalette.ImageBlockColor);
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.TopLeftRectangle));
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.BottomRightRectangle));
            }
        }
        private void SetLabelForIntersectingBlocks(Rectangle pbRectangle, OcrBlockLabel? label)
        {
            var bookFolder = Path.GetDirectoryName(csvFile_tb.Text);
            var jsonFolder = Path.Combine(bookFolder, "json");
            var fileName = ocrBlock_lb.SelectedItem as string;

            if (fileName == null) return;

            var jsonFile = Directory.GetFiles(jsonFolder).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
            var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];
            foreach (var block in page.Blocks)
            {
                if (block.BoundingBox.Rectangle().IntersectsWith(originalRect))
                {
                    var row = _blockRows.First(r => r.FileName == fileName && r.BlockIndex == page.Blocks.IndexOf(block));
                    row.Label = label;
                }
            }

            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = fileName;
        }

        private void AddImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;
            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var imageBlock = new ImageBlock
            {
                FileName = fileName,
                TopLeftX = originalRect.X,
                TopLeftY = originalRect.Y,
                BottomRightX = originalRect.X + originalRect.Size.Width,
                BottomRightY = originalRect.Y + originalRect.Size.Height
            };

            if (_imageBlocks == null) _imageBlocks = new List<ImageBlock>();

            _imageBlocks.Add(imageBlock);
            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = fileName;
        }

        private void RemoveImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;
            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            _imageBlocks?.RemoveAll(ib => ib.FileName == fileName && ib.Rectangle.IntersectsWith(originalRect));

            pictureBox1.Refresh();
        }

        private void LoadPages_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load ocr block CSV",
                Filter = "CSV|*.csv"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _loadPages = true;

            csvFile_tb.Text = dialog.FileName;

            using (var csv = new CsvReader(new StreamReader(dialog.FileName), CultureInfo.InvariantCulture))
            {
                _blockRows = csv.GetRecords<OcrBlockRow>().OrderBy(r => r.ImageIndex).ToList();
            }

            ocrBlock_lb.Items.Clear();
            var fileNames = _blockRows.Select(r => r.FileName).Distinct().ToList();
            foreach (var fileName in fileNames)
            {
                ocrBlock_lb.Items.Add(fileName);
            }
        }

        private void LoadBlocks_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load ocr block CSV",
                Filter = "CSV|*.csv"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _loadPages = false;

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

        private void OcrBlock_lbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (_loadPages) return;

            var row = ocrBlock_lb.SelectedItem as OcrBlockRow;

            if (row == null) return;

            if (e.KeyCode == Keys.P) row.Label = OcrBlockLabel.Paragraph;

            if (e.KeyCode == Keys.C) row.Label = OcrBlockLabel.Comment;

            if (e.KeyCode == Keys.T) row.Label = OcrBlockLabel.Title;

            if (e.KeyCode == Keys.G) row.Label = OcrBlockLabel.Garbage;

            if (e.KeyCode == Keys.I) row.Label = OcrBlockLabel.Image;

            if (e.KeyCode == Keys.N) row.Label = null;

            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = row;
        }

        private void OcrBlock_lbOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                var bookFolder = Path.GetDirectoryName(csvFile_tb.Text);
                var imageFolder = Path.Combine(bookFolder, "images");
                var jsonFolder = Path.Combine(bookFolder, "json");
                var fileName = _loadPages
                    ? ocrBlock_lb.SelectedItem as string
                    : (ocrBlock_lb.SelectedItem as OcrBlockRow)?.FileName;

                if (fileName == null) return;

                var imageFile = Directory.GetFiles(imageFolder).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
                if (imageFile == null)
                {
                    MessageBox.Show("Image file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var jsonFile = Directory.GetFiles(jsonFolder).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
                if (jsonFile == null)
                {
                    MessageBox.Show("Json file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var image = new Bitmap(Image.FromFile(imageFile));
                var ocrResponse = JsonConvert.DeserializeObject<OcrResponse>(File.ReadAllText(jsonFile));
                var page = ocrResponse.Results[0].Results[0].TextDetection.Pages[0];

                var drawRows = _loadPages
                    ? _blockRows.Where(r => r.FileName == fileName).ToList()
                    : new List<OcrBlockRow> { ocrBlock_lb.SelectedItem as OcrBlockRow };

                foreach (var row in drawRows)
                {
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
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                throw;
            }
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

            var labeledRows = _blockRows.Where(r => r.Label != null).ToList();
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

            using (var csv = new CsvWriter(new StreamWriter(dialog.FileName), CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(_blockRows);
            }

            MessageBox.Show($"Saved {_blockRows.Count} to '{dialog.FileName}'", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
