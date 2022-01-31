using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeninSearch.Ocr.Labeling;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.YandexVision;
using Newtonsoft.Json;

namespace LeninSearch.Ocr
{
    public partial class LabelingForm : Form
    {
        private bool _loadPages;

        private Point? _selectionStartPoint;

        private OcrData _ocrData = OcrData.Empty();

        private Action<Point> _moveImageBlockDragPoint;

        private List<OcrFeaturedBlock> _featuredBlocks;

        public LabelingForm()
        {
            InitializeComponent();

            displayBlocks_btn.Click += DisplayBlocks_btnOnClick;
            displayPages_btn.Click += DisplayPages_btnOnClick;

            ocrBlock_lb.SelectedIndexChanged += OcrBlock_lbOnSelectedIndexChanged;
            ocrBlock_lb.KeyDown += OcrBlock_lbOnKeyDown;
            trainModel_btn.Click += TrainModel_miOnClick;

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

            generateImageBlocks_btn.Click += GenerateImageBlocksClick;
            saveOcrData_btn.Click += SaveOcrDataClick;

            openBookFolder_btn.Click += OpenBookFolder_btnOnClick;
            generateBlocks_btn.Click += GenerateBlocks_btnOnClick;
        }

        private async void GenerateBlocks_btnOnClick(object? sender, EventArgs e)
        {
            var imageFiles = Directory.GetFiles(bookFolder_tb.Text)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")).ToList();

            var blockService = new YandexVisionOcrBlockService();

            _featuredBlocks = new List<OcrFeaturedBlock>();

            var sw = new Stopwatch(); sw.Start();

            var chunks = imageFiles.ChunkBy(8);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = chunks.Count;
            progressBar1.Value = 0;

            for (var i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];

                var tasks = chunk.Select(f => blockService.GetBlocksAsync(f));

                var results = await Task.WhenAll(tasks);
                foreach (var result in results)
                {
                    if (!result.Success)
                    {
                        MessageBox.Show(result.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _featuredBlocks.AddRange(result.Blocks);
                }

                progressBar1.Value = i + 1;
            }

            sw.Stop();

            MessageBox.Show($"Blocks generated in {sw.Elapsed}", "Blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenBookFolder_btnOnClick(object? sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            bookFolder_tb.Text = dialog.SelectedPath;
        }

        private void TrainModel_miOnClick(object? sender, EventArgs e)
        {
            //var labeledCount = _blockRows?.Count(r => r.Label.HasValue) ?? 0;

            //if (DialogResult.Yes != MessageBox.Show($"Train model base on {labeledCount} blocks?", "Model",
            //    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)) return;

            //var teacher = new RandomForestLearning {NumberOfTrees = 10};



        }

        private void SaveOcrDataClick(object? sender, EventArgs e)
        {
            var ocrDataFile = Path.Combine(bookFolder_tb.Text, "ocr-data.json");

            if (File.Exists(ocrDataFile) && DialogResult.Yes != MessageBox.Show("Ocr data exists, overwrite?", "Ocr data",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }

            var ocrDataJson = JsonConvert.SerializeObject(_featuredBlocks);

            File.WriteAllText(ocrDataFile, ocrDataJson);

            MessageBox.Show($"Ocr data saved to '{ocrDataFile}'", "Ocr data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateImageBlocksClick(object sender, EventArgs e)
        {
            // 1. get image row groups
            var fileBlockGroups = new Dictionary<string, List<List<OcrFeaturedBlock>>>();
            var fileNames = _featuredBlocks.OrderBy(r => r.ImageIndex).Select(r => r.FileName).Distinct().ToList();
            foreach (var fileName in fileNames)
            {
                var blockGroups = new List<List<OcrFeaturedBlock>>();
                var blockGroup = new List<OcrFeaturedBlock>();
                var blocks = _featuredBlocks.Where(r => r.FileName == fileName).OrderBy(r => r.BlockIndex).ToList();

                foreach (var block in blocks)
                {
                    if (block.Label != OcrBlockLabel.Image && blockGroup.Any())
                    {
                        blockGroups.Add(blockGroup);
                        blockGroup = new List<OcrFeaturedBlock>();
                    }
                    else if (block.Label == OcrBlockLabel.Image)
                    {
                        blockGroup.Add(block);
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
            _ocrData.ImageBlocks = new List<OcrImageBlock>();
            foreach (var fileName in fileBlockGroups.Keys)
            {
                var imageFile = Directory.GetFiles(bookFolder_tb.Text).First(f => Path.GetFileNameWithoutExtension(f) == fileName);
                using var image = Image.FromFile(imageFile);

                foreach (var blockGroup in fileBlockGroups[fileName])
                {
                    var topY = int.MaxValue;
                    var leftX = int.MaxValue;
                    var bottomY = 0;
                    var rightX = 0;

                    foreach (var block in blockGroup)
                    {
                        if (block.TopLeftX < leftX) leftX = block.TopLeftX;
                        if (block.TopLeftY < topY) topY = block.TopLeftY;
                        if (block.BottomRightX > rightX) rightX = block.BottomRightX;
                        if (block.BottomRightY > bottomY) bottomY = block.BottomRightY;
                    }

                    var defaultLeft = 40;
                    if (leftX > defaultLeft) leftX = defaultLeft;

                    var defaultRight = image.Width - defaultLeft;
                    if (rightX < defaultRight) rightX = defaultRight;

                    var imageBlock = new OcrImageBlock
                    {
                        FileName = fileName,
                        TopLeftX = leftX,
                        TopLeftY = topY - 10,
                        BottomRightX = rightX,
                        BottomRightY = bottomY + 10
                    };

                    _ocrData.ImageBlocks.Add(imageBlock);
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
                    var fileImageBlocks = _ocrData.ImageBlocks.Where(ib => ib.FileName == fileName).ToList();
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

            var fileImageBlocks = _ocrData.ImageBlocks.Where(b => b.FileName == fileName).ToList();
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
            var fileName = ocrBlock_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var fileBlocks = _featuredBlocks.Where(b => b.FileName == fileName).ToList();
            foreach (var block in fileBlocks)
            {
                if (block.Rectangle.IntersectsWith(originalRect))
                {
                    block.Label = label;
                }
            }

            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = fileName;
        }

        private void AddImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;
            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var imageBlock = new OcrImageBlock
            {
                FileName = fileName,
                TopLeftX = originalRect.X,
                TopLeftY = originalRect.Y,
                BottomRightX = originalRect.X + originalRect.Size.Width,
                BottomRightY = originalRect.Y + originalRect.Size.Height
            };

            _ocrData.ImageBlocks.Add(imageBlock);
            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = fileName;
        }

        private void RemoveImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;
            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            _ocrData.ImageBlocks.RemoveAll(ib => ib.FileName == fileName && ib.Rectangle.IntersectsWith(originalRect));

            pictureBox1.Refresh();
        }

        private void DisplayPages_btnOnClick(object? sender, EventArgs e)
        {
            ocrBlock_lb.Items.Clear();
            var fileNames = _featuredBlocks.Select(r => r.FileName).Distinct().ToList();
            foreach (var fileName in fileNames)
            {
                ocrBlock_lb.Items.Add(fileName);
            }
        }

        private void DisplayBlocks_btnOnClick(object? sender, EventArgs e)
        {
            ocrBlock_lb.Items.Clear();
            foreach (var block in _featuredBlocks)
            {
                ocrBlock_lb.Items.Add(block);
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
                var fileName = _loadPages
                    ? ocrBlock_lb.SelectedItem as string
                    : (ocrBlock_lb.SelectedItem as OcrBlockRow)?.FileName;

                if (fileName == null) return;

                var imageFile = Directory.GetFiles(bookFolder_tb.Text).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
                if (imageFile == null)
                {
                    MessageBox.Show("Image file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var image = new Bitmap(Image.FromFile(imageFile));
                var drawBlocks = _loadPages
                    ? _featuredBlocks.Where(r => r.FileName == fileName).ToList()
                    : new List<OcrFeaturedBlock> { ocrBlock_lb.SelectedItem as OcrFeaturedBlock };

                foreach (var block in drawBlocks)
                {
                    using (var g = Graphics.FromImage(image))
                    {
                        var pen = GetPen(block);
                        g.DrawRectangle(pen, block.Rectangle);
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

        private Pen GetPen(OcrFeaturedBlock block)
        {
            var color = BlockPalette.GetColor(block.Label);

            return new Pen(color, 4);
        }
    }
}
