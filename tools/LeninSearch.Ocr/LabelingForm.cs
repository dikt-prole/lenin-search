using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.MachineLearning.DecisionTrees;
using LeninSearch.Ocr.Labeling;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.YandexVision;
using Newtonsoft.Json;

namespace LeninSearch.Ocr
{
    public partial class LabelingForm : Form
    {
        private bool _displayPages;

        private Point? _selectionStartPoint;

        private OcrData _ocrData = OcrData.Empty();

        private Action<Point> _moveImageBlockDragPoint;

        private RandomForest _model;

        public LabelingForm()
        {
            InitializeComponent();

            displayBlocks_btn.Click += DisplayBlocksOnClick;
            displayPages_btn.Click += DisplayPagesClick;

            ocrBlock_lb.SelectedIndexChanged += OcrBlock_lbOnSelectedIndexChanged;
            ocrBlock_lb.KeyDown += OcrBlock_lbOnKeyDown;
            trainModel_btn.Click += TrainModelClick;
            applyModel_btn.Click += ApplyModelClick;

            paragraph_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Paragraph);
            continuation_panel.BackColor = BlockPalette.GetColor(OcrBlockLabel.Paragraph);
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

            openBookFolder_btn.Click += OpenBookFolderClick;
            generateBlocks_btn.Click += GenerateBlocksClick;
        }

        private void ApplyModelClick(object? sender, EventArgs e)
        {
            if (_model == null)
            {
                MessageBox.Show("No model trained yet", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dialog = new ImageScopeDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var featuredBlocks = _ocrData.FeaturedBlocks
                .Where(b => b.Features != null)
                .Where(dialog.BlockMatch)
                .ToList();

            var inputs = featuredBlocks.Select(b => b.Features.ToFeatureRow()).ToArray();
            var predicted = _model.Decide(inputs);

            for (var i = 0; i < featuredBlocks.Count; i++)
            {
                featuredBlocks[i].Label = (OcrBlockLabel)predicted[i];
            }

            MessageBox.Show("Model applied", "Model", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void GenerateBlocksClick(object? sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = GetImageFiles(bookFolder_tb.Text)
                .Where(f => dialog.ImageMatch(int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray()))))
                .ToList();

            var blockService = new YandexVisionOcrBlockService();

            _ocrData.FeaturedBlocks = new List<OcrFeaturedBlock>();

            var sw = new Stopwatch(); sw.Start();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Count;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Count; i++)
            {
                var imageFile = imageFiles[i];

                var result = await blockService.GetBlocksAsync(imageFile);
                if (!result.Success)
                {
                    MessageBox.Show(result.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _ocrData.FeaturedBlocks.AddRange(result.Blocks);

                progressBar1.Value = i + 1;
            }

            sw.Stop();

            MessageBox.Show($"Blocks generated in {sw.Elapsed}", "Blocks", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DisplayBlocksOnClick(null, null);

            ocrBlock_lb.SelectedIndex = 0;
        }

        private void OpenBookFolderClick(object? sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _ocrData = OcrData.Load(dialog.SelectedPath);

            bookFolder_tb.Text = dialog.SelectedPath;
        }

        private void TrainModelClick(object? sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var labeledBlocks = _ocrData.FeaturedBlocks
                .Where(r => r.Label.HasValue && r.Features != null)
                .Where(dialog.BlockMatch).ToList();

            double[][] inputs = labeledBlocks.Select(lb => lb.Features.ToFeatureRow()).ToArray();
            int[] outputs = labeledBlocks.Select(lb => (int)lb.Label).ToArray();

            Accord.Math.Random.Generator.Seed = 1;
            var teacher = new RandomForestLearning
            {
                NumberOfTrees = 20
            };

            _model = teacher.Learn(inputs, outputs);

            MessageBox.Show($"Model trained on {labeledBlocks.Count} examples", "Model", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveOcrDataClick(object? sender, EventArgs e)
        {
            _ocrData.Save(bookFolder_tb.Text);

            MessageBox.Show("Ocr data saved", "Ocr data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateImageBlocksClick(object sender, EventArgs e)
        {
            // 1. get image row groups
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var fileBlockGroups = new Dictionary<string, List<List<OcrFeaturedBlock>>>();
            var fileNames = _ocrData.FeaturedBlocks.Where(dialog.BlockMatch)
                .OrderBy(r => r.ImageIndex).Select(r => r.FileName).Distinct().ToList();
            foreach (var fileName in fileNames)
            {
                var blockGroups = new List<List<OcrFeaturedBlock>>();
                var blockGroup = new List<OcrFeaturedBlock>();
                var blocks = _ocrData.FeaturedBlocks.Where(r => r.FileName == fileName).OrderBy(r => r.BlockIndex).ToList();

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
            if (_displayPages && pictureBox1.Image != null)
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
            if (_displayPages)
            {
                if (e.Button == MouseButtons.Right && _selectionStartPoint != null)
                {
                    var xs = new List<int> { _selectionStartPoint.Value.X, e.Location.X }.OrderBy(i => i).ToList();
                    var ys = new List<int> { _selectionStartPoint.Value.Y, e.Location.Y }.OrderBy(i => i).ToList();
                    var rect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                    var menu = new ContextMenuStrip();
                    menu.Items.Add("Image", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Image));
                    menu.Items.Add("Paragraph", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Paragraph));
                    menu.Items.Add("Continuation", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Continuation));
                    menu.Items.Add("Comment", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Comment));
                    menu.Items.Add("Title", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Title));
                    menu.Items.Add("Garbage", null, (o, a) => SetLabelForIntersectingBlocks(rect, OcrBlockLabel.Garbage));
                    menu.Items.Add("None", null, (o, a) => SetLabelForIntersectingBlocks(rect, null));
                    menu.Items.Add("Add Image Block", null, (o, a) => AddImageBlock(rect));
                    menu.Items.Add("Remove Image Block", null, (o, a) => RemoveImageBlock(rect));
                    menu.Items.Add("Break Block Into Lines", null, (o, a) => BreakBlockIntoLines(rect));
                    menu.Items.Add("Merge Blocks", null, (o, a) => MergeBlocks(rect));
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
        private void BreakBlockIntoLines(Rectangle pbRectangle)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var fileBlocks = _ocrData.FeaturedBlocks.Where(b => b.FileName == fileName).ToList();
            var intersectingBlocks = fileBlocks.Where(b => b.Rectangle.IntersectsWith(originalRect)).ToList();
            foreach (var block in intersectingBlocks)
            {
                var blockIndex = _ocrData.FeaturedBlocks.IndexOf(block);
                _ocrData.FeaturedBlocks.Remove(block);
                for (var i = block.Lines.Count - 1; i >= 0; i--)
                {
                    var line = block.Lines[i];
                    var lineBlock = new OcrFeaturedBlock
                    {
                        FileName = fileName,
                        BottomRightX = line.BottomRightX,
                        BottomRightY = line.BottomRightY,
                        TopLeftX = line.TopLeftX,
                        TopLeftY = line.TopLeftY,
                        Features = block.Features.Copy(),
                        Label = block.Label,
                        Lines = new List<OcrLine> {line}
                    };
                    _ocrData.FeaturedBlocks.Insert(blockIndex, lineBlock);
                }
            }

            fileBlocks = _ocrData.FeaturedBlocks.Where(b => b.FileName == fileName).ToList();
            for (var i = 0; i < fileBlocks.Count; i++)
            {
                fileBlocks[i].BlockIndex = i;
            }

            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = fileName;
        }

        private void MergeBlocks(Rectangle pbRectangle)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var fileBlocks = _ocrData.FeaturedBlocks.Where(b => b.FileName == fileName).ToList();
            var intersectingBlocks = fileBlocks.Where(b => b.Rectangle.IntersectsWith(originalRect)).ToList();

            if (intersectingBlocks.Count < 2) return;

            var firstIntersectingBlock = intersectingBlocks.First();
            var firstIntersectingBlockIndex = _ocrData.FeaturedBlocks.IndexOf(firstIntersectingBlock);

            foreach (var block in intersectingBlocks)
            {
                _ocrData.FeaturedBlocks.Remove(block);
            }

            var lines = intersectingBlocks.SelectMany(b => b.Lines).ToList();

            var topLeftX = intersectingBlocks.Select(b => b.TopLeftX).Min();
            var topLeftY = intersectingBlocks.Select(b => b.TopLeftY).Min();
            var bottomRightX = intersectingBlocks.Select(b => b.BottomRightX).Max();
            var bottomRightY = intersectingBlocks.Select(b => b.BottomRightY).Max();
            
            var resultBlock = new OcrFeaturedBlock
            {
                FileName = fileName,
                TopLeftX = topLeftX,
                TopLeftY = topLeftY,
                BottomRightX = bottomRightX,
                BottomRightY = bottomRightY,
                Features = firstIntersectingBlock.Features,
                Label = firstIntersectingBlock.Label,
                Lines = lines
            };
            _ocrData.FeaturedBlocks.Insert(firstIntersectingBlockIndex, resultBlock);

            fileBlocks = _ocrData.FeaturedBlocks.Where(b => b.FileName == fileName).ToList();
            for (var i = 0; i < fileBlocks.Count; i++)
            {
                fileBlocks[i].BlockIndex = i;
            }

            ocrBlock_lb.Items[ocrBlock_lb.SelectedIndex] = fileName;
        }
        private void SetLabelForIntersectingBlocks(Rectangle pbRectangle, OcrBlockLabel? label)
        {
            var fileName = ocrBlock_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var fileBlocks = _ocrData.FeaturedBlocks.Where(b => b.FileName == fileName).ToList();
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

        private void DisplayPagesClick(object? sender, EventArgs e)
        {
            ocrBlock_lb.Items.Clear();

            var fileNames = GetImageFiles(bookFolder_tb.Text)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(f => int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray())))
                .ToList();

            foreach (var fileName in fileNames)
            {
                ocrBlock_lb.Items.Add(fileName);
            }

            _displayPages = true;
        }

        private void DisplayBlocksOnClick(object? sender, EventArgs e)
        {
            ocrBlock_lb.Items.Clear();
            foreach (var block in _ocrData.FeaturedBlocks)
            {
                ocrBlock_lb.Items.Add(block);
            }

            _displayPages = false;
        }

        private void OcrBlock_lbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (_displayPages) return;

            var row = ocrBlock_lb.SelectedItem as OcrFeaturedBlock;

            if (row == null) return;

            if (e.KeyCode == Keys.P) row.Label = OcrBlockLabel.Paragraph;

            if (e.KeyCode == Keys.O) row.Label = OcrBlockLabel.Continuation;

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
                var fileName = _displayPages
                    ? ocrBlock_lb.SelectedItem as string
                    : (ocrBlock_lb.SelectedItem as OcrFeaturedBlock)?.FileName;

                if (fileName == null) return;

                var imageFile = Directory.GetFiles(bookFolder_tb.Text).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
                if (imageFile == null)
                {
                    MessageBox.Show("Image file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var image = new Bitmap(Image.FromFile(imageFile));
                var drawBlocks = _displayPages
                    ? _ocrData.FeaturedBlocks.Where(r => r.FileName == fileName).ToList()
                    : new List<OcrFeaturedBlock> { ocrBlock_lb.SelectedItem as OcrFeaturedBlock };

                foreach (var block in drawBlocks)
                {
                    using (var g = Graphics.FromImage(image))
                    {
                        using var brush = GetBrush(block);
                        g.FillRectangle(brush, block.Rectangle);

                        using var pen = GetPen(block);
                        g.DrawRectangle(pen, block.Rectangle);
                        
                        using var textBrush = new SolidBrush(Color.DarkViolet);
                        var font = new Font(Font.FontFamily, 12, FontStyle.Bold);
                        g.DrawString(block.BlockIndex.ToString(), font, textBrush, block.BottomRightX + 1, block.TopLeftY);
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

            return new Pen(color, 2);
        }

        private Brush GetBrush(OcrFeaturedBlock block)
        {
            var color = BlockPalette.GetColor(block.Label);

            return new SolidBrush(Color.FromArgb(64, color));
        }

        private string[] GetImageFiles(string folder)
        {
            return Directory.GetFiles(folder)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .ToArray();
        }
    }
}
