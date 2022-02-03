using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.MachineLearning.DecisionTrees;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.YandexVision;

namespace LeninSearch.Ocr
{
    public partial class LabelingForm : Form
    {
        private bool _displayPages;

        private Point? _selectionStartPoint;

        private OcrData _ocrData = OcrData.Empty();

        private Action<Point> _moveImageBlockDragPoint;

        private RandomForest _model;

        private OcrLabel? _mouseLeftLabel;

        private readonly Dictionary<Keys, OcrLabel?> _keyLabels = new Dictionary<Keys, OcrLabel?>
        {
            {Keys.S, OcrLabel.PStart},
            {Keys.M, OcrLabel.PMiddle},
            {Keys.E, OcrLabel.PEnd},
            {Keys.C, OcrLabel.Comment},
            {Keys.T, OcrLabel.Title},
            {Keys.A, OcrLabel.Image},
            {Keys.G, OcrLabel.Garbage},
            {Keys.N, null}
        };

        public LabelingForm()
        {
            InitializeComponent();

            displayPages_btn.Click += DisplayPagesClick;

            ocr_lb.SelectedIndexChanged += OcrLbOnSelectedIndexChanged;
            ocr_lb.KeyDown += OcrLbOnKeyDown;
            trainModel_btn.Click += TrainModelClick;
            applyModel_btn.Click += ApplyModelClick;

            none_panel.BackColor = OcrPalette.GetColor(null);
            pstart_panel.BackColor = OcrPalette.GetColor(OcrLabel.PStart);
            pmiddle_panel.BackColor = OcrPalette.GetColor(OcrLabel.PMiddle);
            pend_panel.BackColor = OcrPalette.GetColor(OcrLabel.PEnd);
            comment_panel.BackColor = OcrPalette.GetColor(OcrLabel.Comment);
            title_panel.BackColor = OcrPalette.GetColor(OcrLabel.Title);
            image_panel.BackColor = OcrPalette.GetColor(OcrLabel.Image);
            garbage_panel.BackColor = OcrPalette.GetColor(OcrLabel.Garbage);

            pictureBox1.Paint += PictureBox1OnPaint;
            pictureBox1.MouseDown += PictureBox1OnMouseDown;
            pictureBox1.MouseUp += PictureBox1OnMouseUp;
            pictureBox1.MouseMove += PictureBox1OnMouseMove;

            generateImageBlocks_btn.Click += GenerateImageBlocksClick;
            saveOcrData_btn.Click += SaveOcrDataClick;

            openBookFolder_btn.Click += OpenBookFolderClick;
            generateLines_btn.Click += GenerateLinesClick;
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

            var featuredLines = _ocrData.Pages.SelectMany(p => p.Lines)
                .Where(b => b.Features != null)
                .Where(dialog.LineMatch)
                .ToList();

            var inputs = featuredLines.Select(l => l.Features.ToFeatureRow()).ToArray();
            var predicted = _model.Decide(inputs);

            for (var i = 0; i < featuredLines.Count; i++)
            {
                featuredLines[i].Label = (OcrLabel)predicted[i];
            }

            MessageBox.Show("Model applied", "Model", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void GenerateLinesClick(object? sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = GetImageFiles(bookFolder_tb.Text)
                .Where(f => dialog.ImageMatch(int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray()))))
                .ToList();

            var lineService = new YandexVisionOcrLineService();

            _ocrData.Pages = new List<OcrPage>();

            var sw = new Stopwatch(); sw.Start();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Count;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Count; i++)
            {
                var imageFile = imageFiles[i];

                var result = await lineService.GetOcrPageAsync(imageFile);
                if (!result.Success)
                {
                    MessageBox.Show(result.Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _ocrData.Pages.Add(result.Page);

                progressBar1.Value = i + 1;
            }

            sw.Stop();

            MessageBox.Show($"Lines generated in {sw.Elapsed}", "Lines", MessageBoxButtons.OK, MessageBoxIcon.Information);

            DisplayPagesClick(null, null);

            ocr_lb.SelectedIndex = 0;
        }

        private void OpenBookFolderClick(object? sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _ocrData = OcrData.Load(dialog.SelectedPath);

            bookFolder_tb.Text = dialog.SelectedPath;

            DisplayPagesClick(null, null);
        }

        private void TrainModelClick(object? sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var labeledLines = _ocrData.Pages.SelectMany(p => p.Lines)
                .Where(r => r.Label.HasValue && r.Features != null)
                .Where(dialog.LineMatch).ToList();

            double[][] inputs = labeledLines.Select(l => l.Features.ToFeatureRow()).ToArray();
            int[] outputs = labeledLines.Select(l => (int)l.Label).ToArray();

            Accord.Math.Random.Generator.Seed = 1;
            var teacher = new RandomForestLearning
            {
                NumberOfTrees = 20
            };

            _model = teacher.Learn(inputs, outputs);

            MessageBox.Show($"Model trained on {labeledLines.Count} examples", "Model", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            var fileLineGroups = new Dictionary<string, List<List<OcrLine>>>();
            var fileNames = _ocrData.Pages.SelectMany(p => p.Lines).Where(dialog.LineMatch)
                .OrderBy(r => r.ImageIndex).Select(r => r.FileName).Distinct().ToList();
            foreach (var fileName in fileNames)
            {
                var lineGroups = new List<List<OcrLine>>();
                var lineGroup = new List<OcrLine>();
                var lines = _ocrData.GetPage(fileName).Lines.OrderBy(r => r.LineIndex).ToList();

                foreach (var line in lines)
                {
                    if (line.Label != OcrLabel.Image && lineGroup.Any())
                    {
                        lineGroups.Add(lineGroup);
                        lineGroup = new List<OcrLine>();
                    }
                    else if (line.Label == OcrLabel.Image)
                    {
                        lineGroup.Add(line);
                    }
                }

                if (lineGroup.Any())
                {
                    lineGroups.Add(lineGroup);
                }

                if (lineGroups.Any())
                {
                    fileLineGroups.Add(fileName, lineGroups);
                }
            }

            // 2. generate image lines
            _ocrData.ImageBlocks = new List<OcrImageBlock>();
            foreach (var fileName in fileLineGroups.Keys)
            {
                var imageFile = Directory.GetFiles(bookFolder_tb.Text).First(f => Path.GetFileNameWithoutExtension(f) == fileName);
                using var image = Image.FromFile(imageFile);

                foreach (var lineGroup in fileLineGroups[fileName])
                {
                    var topY = int.MaxValue;
                    var leftX = int.MaxValue;
                    var bottomY = 0;
                    var rightX = 0;

                    foreach (var line in lineGroup)
                    {
                        if (line.TopLeftX < leftX) leftX = line.TopLeftX;
                        if (line.TopLeftY < topY) topY = line.TopLeftY;
                        if (line.BottomRightX > rightX) rightX = line.BottomRightX;
                        if (line.BottomRightY > bottomY) bottomY = line.BottomRightY;
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

            MessageBox.Show("Image lines were auto generated", "Image lines", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                if (e.Button == MouseButtons.Left && ocr_lb.SelectedItem is string fileName)
                {
                    var originalPoint = pictureBox1.ToOriginalPoint(e.Location);
                    var fileImageBlocks = _ocrData.ImageBlocks
                        .Where(ib => ib.FileName == fileName)
                        .Where(ib => ib.TopLeftRectangle.Contains(originalPoint) || ib.BottomRightRectangle.Contains(originalPoint))
                        .ToList();
                    _moveImageBlockDragPoint = null;
                    if (fileImageBlocks.Any())
                    {
                        foreach (var ib in fileImageBlocks)
                        {
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
                    else
                    {
                        var page = _ocrData.GetPage(fileName);
                        var linesAtPoint = page.Lines.Where(b => b.Rectangle.Contains(originalPoint)).ToList();
                        foreach (var line in linesAtPoint)
                        {
                            line.Label = _mouseLeftLabel;
                        }

                        ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
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
                    menu.Items.Add("PStart", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.PStart));
                    menu.Items.Add("PMiddle", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.PMiddle));
                    menu.Items.Add("PEnd", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.PEnd));
                    menu.Items.Add("Comment", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Comment));
                    menu.Items.Add("Title", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Title));
                    menu.Items.Add("Image", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Image));
                    menu.Items.Add("Garbage", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Garbage));
                    menu.Items.Add("None", null, (o, a) => SetLabelForIntersectingLines(rect, null));
                    menu.Items.Add(new ToolStripSeparator());
                    menu.Items.Add("Set Top Divider", null, (o, a) => SetTopDivider(rect));
                    menu.Items.Add("Set Bottom Divider", null, (o, a) => SetBottomDivider(rect));
                    menu.Items.Add(new ToolStripSeparator());
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
            var fileName = ocr_lb.SelectedItem as string;

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
                using var ibPen = new Pen(OcrPalette.ImageBlockColor, 2);
                e.Graphics.DrawRectangle(ibPen, pictureBox1.ToPictureBoxRectangle(ib.Rectangle));

                using var ibBrush = new SolidBrush(OcrPalette.ImageBlockColor);
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.TopLeftRectangle));
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.BottomRightRectangle));
            }
        }

        private void SetLabelForIntersectingLines(Rectangle pbRectangle, OcrLabel? label)
        {
            var fileName = ocr_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            foreach (var line in _ocrData.GetPage(fileName).Lines)
            {
                if (line.Rectangle.IntersectsWith(originalRect))
                {
                    line.Label = label;
                }
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void SetTopDivider(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var divider = new DividerLine(originalRect.Y, 10, pictureBox1.Image.Width - 10);

            var page = _ocrData.GetPage(fileName);

            if (page == null) return;

            page.TopDivider = divider;

            foreach (var line in page.Lines)
            {
                if (line.Features == null) continue;

                line.Features.BelowTopDivider = line.TopLeftY > divider.Y ? 1 : 0;
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void SetBottomDivider(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var divider = new DividerLine(originalRect.Y, 10, pictureBox1.Image.Width - 10);

            var page = _ocrData.GetPage(fileName);

            if (page == null) return;

            page.BottomDivider = divider;

            foreach (var line in page.Lines)
            {
                if (line.Features == null) continue;

                line.Features.AboveBottomDivider = line.TopLeftY < divider.Y ? 1 : 0;
            }

            foreach (var line in page.Lines.Where(l => l.Features?.AboveBottomDivider == 1 && l.Label == OcrLabel.Comment))
            {
                line.Label = null;
            }

            foreach (var line in page.Lines.Where(l => l.Features?.AboveBottomDivider == 0))
            {
                line.Label = OcrLabel.Comment;
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void AddImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
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
            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void RemoveImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            _ocrData.ImageBlocks.RemoveAll(ib => ib.FileName == fileName && ib.Rectangle.IntersectsWith(originalRect));

            pictureBox1.Refresh();
        }

        private void DisplayPagesClick(object? sender, EventArgs e)
        {
            ocr_lb.Items.Clear();

            var fileNames = GetImageFiles(bookFolder_tb.Text)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(f => int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray())))
                .ToList();

            foreach (var fileName in fileNames)
            {
                ocr_lb.Items.Add(fileName);
            }

            _displayPages = true;
        }

        private void OcrLbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_keyLabels.ContainsKey(e.KeyCode)) return;

            var label = _keyLabels[e.KeyCode];

            if (_displayPages)
            {
                _mouseLeftLabel = label;
                SetLabelPanelSelected(label);
            }
            else
            {
                var row = ocr_lb.SelectedItem as OcrLine;
                if (row == null) return;
                row.Label = label;
                ocr_lb.Items[ocr_lb.SelectedIndex] = row;
            }
        }

        private void SetLabelPanelSelected(OcrLabel? label)
        {
            pstart_panel.BorderStyle = label == OcrLabel.PStart
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            pmiddle_panel.BorderStyle = label == OcrLabel.PMiddle
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            pend_panel.BorderStyle = label == OcrLabel.PEnd
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            comment_panel.BorderStyle = label == OcrLabel.Comment
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            title_panel.BorderStyle = label == OcrLabel.Title
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            image_panel.BorderStyle = label == OcrLabel.Image
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            garbage_panel.BorderStyle = label == OcrLabel.Garbage
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
            none_panel.BorderStyle = label == null
                ? BorderStyle.Fixed3D
                : BorderStyle.None;
        }

        private void OcrLbOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                var fileName = ocr_lb.SelectedItem as string;

                if (fileName == null) return;

                var imageFile = Directory.GetFiles(bookFolder_tb.Text).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
                if (imageFile == null)
                {
                    MessageBox.Show("Image file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                var image = new Bitmap(Image.FromFile(imageFile));
                var page = _ocrData.GetPage(fileName);

                if (page != null)
                {
                    using var g = Graphics.FromImage(image);
                    using var dividerPen = new Pen(Color.DarkViolet, 2);

                    g.DrawLine(dividerPen, 10, page.TopDivider.Y, image.Width - 10, page.TopDivider.Y);
                    g.DrawLine(dividerPen, 10, page.BottomDivider.Y, image.Width - 10, page.BottomDivider.Y);

                    foreach (var line in page.Lines)
                    {
                        using var brush = GetBrush(line);
                        g.FillRectangle(brush, line.Rectangle);

                        using var pen = GetPen(line);
                        g.DrawRectangle(pen, line.Rectangle);

                        using var textBrush = new SolidBrush(Color.DarkViolet);
                        var font = new Font(Font.FontFamily, 12, FontStyle.Bold);
                        g.DrawString(line.LineIndex.ToString(), font, textBrush, line.BottomRightX + 1, line.TopLeftY);
                    }
                }
                pictureBox1.Image = image;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
                throw;
            }
        }

        private Pen GetPen(OcrLine line)
        {
            var color = OcrPalette.GetColor(line.Label);

            return new Pen(color, 2);
        }

        private Brush GetBrush(OcrLine line)
        {
            var color = OcrPalette.GetColor(line.Label);

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
