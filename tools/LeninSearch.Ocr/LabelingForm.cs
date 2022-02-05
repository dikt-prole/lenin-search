using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Accord.MachineLearning.DecisionTrees;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.Service;
using LeninSearch.Ocr.YandexVision;

namespace LeninSearch.Ocr
{
    public partial class LabelingForm : Form
    {
        private Point? _selectionStartPoint;

        private OcrData _ocrData = OcrData.Empty();

        private Action<Point> _moveImageBlockDragPoint;

        private RandomForest _model;

        private OcrLabel? _mouseLeftLabel;

        private readonly IOcrService _ocrService = new IntersectingLineMergerDecorator(new YandexVisionOcrLineService());
        //private readonly IOcrService _ocrService = new YandexVisionOcrLineService();

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

            saveOcrData_btn.Click += SaveOcrDataClick;

            openBookFolder_btn.Click += OpenBookFolderClick;
            generateLines_btn.Click += GenerateLinesClick;

            rowModel_flp.Controls.Clear();
            var rowModel = OcrLineFeatures.GetDefaultRowModel();
            foreach (var propName in rowModel.Keys)
            {
                var propCheckbox = new CheckBox
                {
                    Text = propName,
                    Checked = rowModel[propName],
                    Width = 200
                };
                rowModel_flp.Controls.Add(propCheckbox);
            }
        }

        private Dictionary<string, bool> GetRowModel()
        {
            return rowModel_flp.Controls.OfType<CheckBox>().ToDictionary(chb => chb.Text, chb => chb.Checked);
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

            var featuredLines = _ocrData.Pages.Where(dialog.PageMatch).SelectMany(p => p.Lines)
                .Where(b => b.Features != null)
                .ToList();

            var rowModel = GetRowModel();

            var inputs = featuredLines.Select(l => l.Features.ToFeatureRow(rowModel)).ToArray();
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

            _ocrData.Pages = new List<OcrPage>();

            var sw = new Stopwatch(); sw.Start();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Count;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Count; i++)
            {
                var imageFile = imageFiles[i];

                var result = await _ocrService.GetOcrPageAsync(imageFile);
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

            var labeledLines = _ocrData.Pages.Where(dialog.PageMatch).SelectMany(p => p.Lines)
                .Where(r => r.Label.HasValue && r.Features != null)
                .ToList();

            var rowModel = GetRowModel();

            double[][] inputs = labeledLines.Select(l => l.Features.ToFeatureRow(rowModel)).ToArray();
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

        private void PictureBox1OnMouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var page = _ocrData.GetPage(filename);
            if (page == null) return;


            if (e.Button == MouseButtons.Right)
            {
                _selectionStartPoint = e.Location;
            }

            if (e.Button == MouseButtons.Left)
            {
                var originalPoint = pictureBox1.ToOriginalPoint(e.Location);
                var imageBlocks = page.ImageBlocks?.Where(ib => 
                    ib.TopLeftRectangle.Contains(originalPoint) || ib.BottomRightRectangle.Contains(originalPoint))
                    .ToList() ?? new List<OcrImageBlock>();
                _moveImageBlockDragPoint = null;
                if (imageBlocks.Any())
                {
                    foreach (var ib in imageBlocks)
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
                    var linesAtPoint = page.Lines.Where(b => b.Rectangle.Contains(originalPoint)).ToList();
                    foreach (var line in linesAtPoint)
                    {
                        line.Label = _mouseLeftLabel;
                    }

                    ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
                }
            }
        }

        private void PictureBox1OnMouseUp(object sender, MouseEventArgs e)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

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
                menu.Items.Add("Merge Lines", null, (o, a) => MergeLines(rect));

                var commentLines = page.Lines.Where(l => l.Label == OcrLabel.Comment).ToList();
                var bindWordMenuItem = new ToolStripMenuItem("Bind Word", null);
                bindWordMenuItem.DropDownItems.Add("None", null, (o, a) => BindWordToCommentLine(rect, null));
                foreach (var commentLine in commentLines)
                {
                    bindWordMenuItem.DropDownItems.Add($"Comment Line {commentLine.LineIndex}", null,
                        (o, a) => BindWordToCommentLine(rect, commentLine.LineIndex));
                }
                menu.Items.Add(bindWordMenuItem);

                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Add Image Block", null, (o, a) => AddImageBlock(rect));
                menu.Items.Add("Remove Image Block", null, (o, a) => RemoveImageBlock(rect));
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add("Show Text", null, (o, a) => SetDisplayText(rect, true));
                menu.Items.Add("Show Features", null, (o, a) => ShowLineFeatures(rect));
                menu.Items.Add("Hide Text", null, (o, a) => SetDisplayText(rect, false));
                menu.Items.Add("Add Word", null, (o, a) => AddWord(rect));

                menu.Show(pictureBox1, e.Location);
                _selectionStartPoint = null;
            }

            if (e.Button == MouseButtons.Left)
            {
                _moveImageBlockDragPoint = null;
                foreach (var imageBlock in page.ImageBlocks)
                {
                    foreach (var line in page.GetIntersectingLines(imageBlock.Rectangle))
                    {
                        line.Label = OcrLabel.Image;
                    }
                }
                ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
            }
        }

        private void PictureBox1OnMouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;

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
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

            if (_selectionStartPoint != null)
            {
                var currentPoint = pictureBox1.PointToClient(Cursor.Position);
                var xs = new List<int> { _selectionStartPoint.Value.X, currentPoint.X }.OrderBy(i => i).ToList();
                var ys = new List<int> { _selectionStartPoint.Value.Y, currentPoint.Y }.OrderBy(i => i).ToList();
                var rect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                e.Graphics.DrawRectangle(Pens.Black, rect);
            }

            foreach (var ib in page.ImageBlocks)
            {
                using var ibPen = new Pen(OcrPalette.ImageBlockColor, 2);
                e.Graphics.DrawRectangle(ibPen, pictureBox1.ToPictureBoxRectangle(ib.Rectangle));

                using var ibBrush = new SolidBrush(OcrPalette.ImageBlockColor);
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.TopLeftRectangle));
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.BottomRightRectangle));
            }
        }

        private void AddWord(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;
            var dialog = new AddWordDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var word = new OcrWord
            {
                TopLeftX = originalRect.X,
                TopLeftY = originalRect.Y,
                BottomRightX = originalRect.X + originalRect.Width,
                BottomRightY = originalRect.Y + originalRect.Height,
                Text = dialog.WordText
            };

            var line = new OcrLine
            {
                TopLeftX = word.TopLeftX,
                TopLeftY = word.TopLeftY,
                BottomRightX = word.BottomRightX,
                BottomRightY = word.BottomRightY,
                Words = new List<OcrWord> { word },
                DisplayText = true
            };

            line.Features = OcrLineFeatures.Calculate(page, line);

            page.Lines.Add(line);

            page.Lines = page.Lines.OrderBy(l => l.TopLeftY).ToList();

            for (var i = 0; i < page.Lines.Count; i++) page.Lines[i].LineIndex = i;

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void SetDisplayText(Rectangle pbRectangle, bool displayText)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            foreach (var line in page.GetIntersectingLines(originalRect)) line.DisplayText = displayText;

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
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

            var page = _ocrData.GetPage(fileName);

            if (page == null) return;

            page.TopDivider = new DividerLine(originalRect.Y, 10, pictureBox1.Image.Width - 10);

            foreach (var line in page.Lines)
            {
                if (line.Features == null) continue;

                line.Features.BelowTopDivider = line.TopLeftY > page.TopDivider.Y ? 1 : 0;

                if (line.Features.BelowTopDivider == 0) line.Label = OcrLabel.Garbage;
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void SetBottomDivider(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;

            if (fileName == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var page = _ocrData.GetPage(fileName);

            if (page == null) return;

            page.BottomDivider = new DividerLine(originalRect.Y, 10, pictureBox1.Image.Width - 10); ;

            foreach (var line in page.Lines)
            {
                if (line.Features == null) continue;

                line.Features.AboveBottomDivider = line.TopLeftY < page.BottomDivider.Y ? 1 : 0;

                if (line.Features.AboveBottomDivider == 1 && line.Label == OcrLabel.Comment) line.Label = null;

                if (line.Features.AboveBottomDivider == 0 && line.Label == null) line.Label = OcrLabel.Comment;
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void AddImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var imageBlock = new OcrImageBlock
            {
                TopLeftX = 20,
                TopLeftY = originalRect.Y,
                BottomRightX = page.Width - 20,
                BottomRightY = originalRect.Y + originalRect.Size.Height
            };

            page.ImageBlocks.Add(imageBlock);

            foreach (var line in page.GetIntersectingLines(imageBlock.Rectangle)) line.Label = OcrLabel.Image;

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private void RemoveImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            page.ImageBlocks.RemoveAll(ib => ib.Rectangle.IntersectsWith(originalRect));

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
        }

        private void OcrLbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_keyLabels.ContainsKey(e.KeyCode)) return;

            var label = _keyLabels[e.KeyCode];

            _mouseLeftLabel = label;
            SetLabelPanelSelected(label);
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
                    using var textBrush = new SolidBrush(Color.DarkViolet);
                    using var commentLinePen = new Pen(OcrPalette.GetColor(OcrLabel.Comment), 1);

                    var font = new Font(Font.FontFamily, 12, FontStyle.Bold);

                    g.DrawLine(dividerPen, 10, page.TopDivider.Y, image.Width - 10, page.TopDivider.Y);
                    g.DrawLine(dividerPen, 10, page.BottomDivider.Y, image.Width - 10, page.BottomDivider.Y);

                    foreach (var line in page.Lines)
                    {
                        using var brush = GetBrush(line);
                        g.FillRectangle(brush, line.Rectangle);

                        

                        using var pen = GetPen(line);
                        g.DrawRectangle(pen, line.Rectangle);

                        g.DrawString(line.LineIndex.ToString(), font, textBrush, line.BottomRightX + 1, line.TopLeftY);

                        foreach (var word in line.Words ?? new List<OcrWord>())
                        {
                            if (word.CommentLineIndex != null)
                            {
                                var commentLine = page.Lines[word.CommentLineIndex.Value];

                                var verticalMinY = word.BottomRightY;
                                var verticalMaxY = commentLine.TopLeftY;
                                var verticalX = (word.BottomRightX + word.TopLeftX) / 2;
                                g.DrawLine(commentLinePen, verticalX, verticalMinY, verticalX, verticalMaxY);

                                var horizontalMinX = word.TopLeftX;
                                var horizontalMaxX = word.BottomRightX;
                                var horizontalY = word.BottomRightY;
                                g.DrawLine(commentLinePen, horizontalMinX, horizontalY, horizontalMaxX, horizontalY);
                            }

                            if (line.DisplayText)
                            {
                                g.DrawString(word.Text, font, textBrush, word.TopLeftX, line.TopLeftY - 15);
                            }
                        }
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

        private void MergeLines(Rectangle pbRectangle)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var intersectingLines = page.Lines.Where(l => l.Rectangle.IntersectsWith(originalRect)).ToList();

            page.MergeLines(intersectingLines[0], intersectingLines.Skip(1).ToArray());

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
        }

        private void BindWordToCommentLine(Rectangle pbRectangle, int? commentLineIndex)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var intersectingLine = page.Lines.FirstOrDefault(l => l.Rectangle.IntersectsWith(originalRect));

            var word = intersectingLine?.Words?.FirstOrDefault(w => w.Rectangle.IntersectsWith(originalRect));

            if (word != null)
            {
                word.CommentLineIndex = commentLineIndex;
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
        }

        private void ShowLineFeatures(Rectangle pbRectangle)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var intersectingLine = page.Lines.FirstOrDefault(l => l.Rectangle.IntersectsWith(originalRect));

            if (intersectingLine == null) return;

            var dialog = new LineFeaturesDialog();

            dialog.SetFeatures(intersectingLine.Features);

            dialog.ShowDialog();
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
