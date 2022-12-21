using Accord.MachineLearning.DecisionTrees;
using LeninSearch.Ocr.CV;
using LeninSearch.Ocr.Model;
using LeninSearch.Ocr.Model.UncoveredContourMatches;
using LeninSearch.Ocr.Service;
using LeninSearch.Ocr.YandexVision;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeninSearch.Ocr
{
    public partial class LabelingForm : Form
    {
        private Point? _selectionStartPoint;

        private OcrData _ocrData = OcrData.Empty();

        private Action<Point> _moveDragPoint;

        private RandomForest _model;

        private OcrLabel? _mouseLeftLabel;

        private readonly IOcrService _ocrService;

        private readonly Dictionary<Keys, OcrLabel?> _keyLabels = new Dictionary<Keys, OcrLabel?>
        {
            {Keys.S, OcrLabel.PStart},
            {Keys.M, OcrLabel.PMiddle},
            {Keys.C, OcrLabel.Comment},
            {Keys.T, OcrLabel.Title},
            {Keys.A, OcrLabel.Image},
            {Keys.G, OcrLabel.Garbage},
            {Keys.N, null}
        };

        private readonly TitleBlockDialog _titleBlockDialog = new TitleBlockDialog();

        private const int MaxLineHeight = 35;

        public LabelingForm()
        {
            InitializeComponent();

            ocr_lb.SelectedIndexChanged += OcrLbOnSelectedIndexChanged;
            ocr_lb.KeyDown += OcrLbOnKeyDown;
            none_panel.BackColor = OcrPalette.GetColor(null);
            pstart_panel.BackColor = OcrPalette.GetColor(OcrLabel.PStart);
            pmiddle_panel.BackColor = OcrPalette.GetColor(OcrLabel.PMiddle);
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

            labeling_rb.Checked = true;

            autoDetectImages_btn.Click += OnAutoDetectImagesClick;

            _ocrService =
                new FeatureSettingDecorator(
                    new IntersectingLineMergerDecorator(
                        new OcrServiceRetryDecorator(
                            new YandexVisionOcrLineService())));
        }

        private void OnAutoDetectImagesClick(object sender, EventArgs e)
        {
            if (_ocrData == null)
            {
                MessageBox.Show("Ocr Data is missing", "Auto Detect Images", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = GetImageFiles(bookFolder_tb.Text)
                .Where(f => dialog.ImageMatch(int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray()))))
                .ToList();

            foreach (var imageFile in imageFiles)
            {
                var imageRectangle = CvUtil.GetImageRectangle(imageFile, MaxLineHeight);

                if (imageRectangle == null) continue;

                var page = _ocrData.GetPage(Path.GetFileNameWithoutExtension(imageFile));

                var imageBlock = new OcrImageBlock
                {
                    TopLeftX = imageRectangle.Value.X,
                    TopLeftY = imageRectangle.Value.Y,
                    BottomRightX = imageRectangle.Value.X + imageRectangle.Value.Width,
                    BottomRightY = imageRectangle.Value.Y + imageRectangle.Value.Size.Height
                };

                page.ImageBlocks.Add(imageBlock);
            }

            MessageBox.Show("Success", "Auto Detect Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static async Task<(List<OcrPage> Pages, bool Success, string Error)> BatchOcr(IOcrService ocrService, List<string> imageFiles, Action<int> progressAction)
        {
            var pages = new List<OcrPage>();
            var batchSize = 10;
            var imageFileBatches = imageFiles.ChunkBy(batchSize);
            for (var batchIndex = 0; batchIndex < imageFileBatches.Count; batchIndex++)
            {
                var batch = imageFileBatches[batchIndex];
                var tasks = batch.Select(ocrService.GetOcrPageAsync);

                var imageFileSw = new Stopwatch(); imageFileSw.Start();

                var results = await Task.WhenAll(tasks);
                progressAction(batchSize * batchIndex + batch.Count);
                if (results.Any(r => !r.Success))
                {
                    return (null, false, results.First(r => !r.Success).Error);
                }
                pages.AddRange(results.Select(r => r.Page));

                imageFileSw.Stop();

                var waitDelta = 1000 - imageFileSw.Elapsed.TotalMilliseconds;
                if (waitDelta > 0)
                {
                    await Task.Delay(1 + (int)waitDelta);
                }
            }

            return (pages, true, null);
        }

        private static async Task<(List<OcrPage> Pages, bool Success, string Error)> SequentialOcr(IOcrService ocrService, List<string> imageFiles, Action<int> progressAction)
        {
            var pages = new List<OcrPage>();
            for (var imageIndex = 0; imageIndex < imageFiles.Count; imageIndex++)
            {
                var result = await ocrService.GetOcrPageAsync(imageFiles[imageIndex]);
                progressAction(imageIndex + 1);
                if (!result.Success)
                {
                    return (null, false, result.Error);
                }
                pages.Add(result.Page);

            }

            return (pages, true, null);
        }

        private async void GenerateLinesClick(object? sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = GetImageFiles(bookFolder_tb.Text)
                .Where(f => dialog.ImageMatch(int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray()))))
                .ToList();

            _ocrData.Pages ??= new List<OcrPage>();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Count;
            progressBar1.Value = 0;

            //var ocrResult = await BatchOcr(_ocrService, imageFiles, progress => progressBar1.Value = progress);
            var ocrResult = await SequentialOcr(_ocrService, imageFiles, progress => progressBar1.Value = progress);
            if (!ocrResult.Success)
            {
                MessageBox.Show(ocrResult.Error, "Ocr Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // replace old pages with new pages
            _ocrData.Pages.RemoveAll(p => ocrResult.Pages.Any(pp => pp.Filename == p.Filename));
            _ocrData.Pages.AddRange(ocrResult.Pages);

            MessageBox.Show("Ocr completed", "Ocr", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void RegeneratePageClick(object sender, EventArgs e)
        {
            await RegeneratePage();
        }

        private async Task RegeneratePage()
        {
            var filename = ocr_lb.SelectedItem as string;
            var imageFile = Directory.GetFiles(bookFolder_tb.Text)
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == filename);
            var ocrResult = await _ocrService.GetOcrPageAsync(imageFile);
            if (!ocrResult.Success)
            {
                MessageBox.Show(ocrResult.Error, "Ocr Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _ocrData.Pages.RemoveAll(p => p.Filename == filename);
            _ocrData.Pages.Add(ocrResult.Page);

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
        }

        private void UncoveredLinksClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var pages = _ocrData.Pages.OrderBy(p => p.ImageIndex)
                .Skip(dialog.MinImageIndex)
                .Take(1 + dialog.MaxImageIndex - dialog.MinImageIndex)
                .ToList();

            // find uncovered contours
            var uncoveredContours = new List<UncoveredContour>();
            foreach (var page in pages)
            {
                var imageFile = Directory.GetFiles(bookFolder_tb.Text)
                    .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == page.Filename);
                uncoveredContours.AddRange(CvUtil.GetUncoveredContours(imageFile, page));
            }

            // find link number contours
            var linkNumberMatch = new CommentLinkNumberMatch();
            var linkNumberContours = uncoveredContours.Where(linkNumberMatch.Match).ToList();
            if (linkNumberContours.Any())
            {
                var linkNumberDialog = new UncoveredContourDialog
                {
                    Text = "Mark uncovered link numbers"
                };
                linkNumberDialog.SetContours(linkNumberContours);
                linkNumberDialog.ShowDialog();
                linkNumberContours = linkNumberContours.Where(lc => !string.IsNullOrEmpty(lc.Word.Text)).ToList();
                foreach (var contour in linkNumberContours)
                {
                    linkNumberMatch.Apply(_ocrData, contour);
                }
            }
            else
            {
                MessageBox.Show("Uncovered link number contours were not found", "Uncovered link number contours",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenBookFolderClick(object? sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _ocrData = OcrData.Load(dialog.SelectedPath);

            bookFolder_tb.Text = dialog.SelectedPath;

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
                var titleBlocks = page.TitleBlocks?.Where(tb =>
                        tb.TopLeftRectangle.Contains(originalPoint) || tb.BottomRightRectangle.Contains(originalPoint))
                    .ToList() ?? new List<OcrTitleBlock>();
                _moveDragPoint = null;
                if (imageBlocks.Any())
                {
                    foreach (var ib in imageBlocks)
                    {
                        if (ib.TopLeftRectangle.Contains(originalPoint))
                        {
                            _moveDragPoint = p =>
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
                            _moveDragPoint = p =>
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
                else if (titleBlocks.Any())
                {
                    foreach (var tb in titleBlocks)
                    {
                        if (tb.TopLeftRectangle.Contains(originalPoint))
                        {
                            _moveDragPoint = p =>
                            {
                                if (ModifierKeys != Keys.Shift)
                                {
                                    tb.TopLeftX = p.X;
                                }
                                tb.TopLeftY = p.Y;
                                pictureBox1.Refresh();
                            };
                            break;
                        }

                        if (tb.BottomRightRectangle.Contains(originalPoint))
                        {
                            _moveDragPoint = p =>
                            {
                                if (ModifierKeys != Keys.Shift)
                                {
                                    tb.BottomRightX = p.X;
                                }
                                tb.BottomRightY = p.Y;
                                pictureBox1.Refresh();
                            };
                            break;
                        }
                    }
                }
                else if (page.TopDivider.DragRectangle.Contains(originalPoint))
                {
                    _moveDragPoint = p =>
                    {
                        page.TopDivider.Y = p.Y;
                        page.CalculateLineDividerFeaturesAndLabels();
                        pictureBox1.Refresh();
                    };
                }
                else if (page.BottomDivider.DragRectangle.Contains(originalPoint))
                {
                    _moveDragPoint = p =>
                    {
                        page.BottomDivider.Y = p.Y;
                        page.CalculateLineDividerFeaturesAndLabels();
                        pictureBox1.Refresh();
                    };
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
                var menu = GetPageMenu(page, rect);
                menu.Show(pictureBox1, e.Location);
                _selectionStartPoint = null;
            }

            if (e.Button == MouseButtons.Left)
            {
                _moveDragPoint = null;
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

        private ContextMenuStrip GetPageMenu(OcrPage page, Rectangle rect)
        {
            var menu = new ContextMenuStrip();

            // 1. labeling section
            if (labeling_rb.Checked || all_rb.Checked)
            {
                menu.Items.Add("PStart", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.PStart));
                menu.Items.Add("PMiddle", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.PMiddle));
                menu.Items.Add("Comment", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Comment));
                menu.Items.Add("Title", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Title));
                menu.Items.Add("Image", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Image));
                menu.Items.Add("Garbage", null, (o, a) => SetLabelForIntersectingLines(rect, OcrLabel.Garbage));
                menu.Items.Add("None", null, (o, a) => SetLabelForIntersectingLines(rect, null));
                if (all_rb.Checked) menu.Items.Add(new ToolStripSeparator());
            }

            // 2. editing section
            if (editing_rb.Checked || all_rb.Checked)
            {
                menu.Items.Add("Break Into Words", null, (o, a) => BreakIntoWords(rect));
                menu.Items.Add("Merge Lines", null, (o, a) => MergeLines(rect));
                menu.Items.Add("Add Word", null, (o, a) => AddWord(rect, false));
                menu.Items.Add("Add Comment Link", null, (o, a) => AddWord(rect, true));
                menu.Items.Add("Set Word Text", null, (o, a) => SetWordText(rect));
                menu.Items.Add("Remove Line", null, (o, a) => RemoveLine(rect));
                menu.Items.Add("Show Features", null, (o, a) => ShowLineFeatures(rect));

                if (all_rb.Checked) menu.Items.Add(new ToolStripSeparator());
            }

            // 3. image blocks section
            if (blocks_rb.Checked || all_rb.Checked)
            {
                menu.Items.Add("Add Image Block", null, (o, a) => AddImageBlock(rect));
                menu.Items.Add("Add Page Wide Image Block", null, (o, a) => AddPageWideImageBlock(rect));
                menu.Items.Add("Remove Image Block", null, (o, a) => RemoveImageBlock(rect));
                menu.Items.Add("Add Title Block", null, (o, a) => AddTitleBlock(rect));
                menu.Items.Add("Remove Title Block", null, (o, a) => RemoveTitleBlock(rect));
            }

            return menu;
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
                _moveDragPoint?.Invoke(originalPoint);
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

            using var ibPen = new Pen(OcrPalette.ImageBlockColor, 2);
            using var ibBrush = new SolidBrush(OcrPalette.ImageBlockColor);
            foreach (var ib in page.ImageBlocks)
            {
                e.Graphics.DrawRectangle(ibPen, pictureBox1.ToPictureBoxRectangle(ib.Rectangle));
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.TopLeftRectangle));
                e.Graphics.FillRectangle(ibBrush, pictureBox1.ToPictureBoxRectangle(ib.BottomRightRectangle));
            }

            using var textBrush = new SolidBrush(Color.DarkViolet);
            using var font = new Font(Font.FontFamily, 12, FontStyle.Bold);
            using var tbPen = new Pen(OcrPalette.TitleBlockColor, 2);
            using var tbBrush = new SolidBrush(OcrPalette.TitleBlockColor);
            foreach (var tb in page.TitleBlocks ?? new List<OcrTitleBlock>())
            {
                var tbpbRect = pictureBox1.ToPictureBoxRectangle(tb.Rectangle);
                e.Graphics.DrawRectangle(tbPen, tbpbRect);
                e.Graphics.FillRectangle(tbBrush, pictureBox1.ToPictureBoxRectangle(tb.TopLeftRectangle));
                e.Graphics.FillRectangle(tbBrush, pictureBox1.ToPictureBoxRectangle(tb.BottomRightRectangle));
                e.Graphics.DrawString($"Level: {tb.TitleLevel}, Text: {tb.TitleText}", font, textBrush, tbpbRect.X, tbpbRect.Y - 20);
            }

            using var dividerPen = new Pen(Color.DarkViolet, 2);
            using var dividerBrush = new SolidBrush(Color.DarkViolet);

            var topDividerStart = pictureBox1.ToPictureBoxPoint(new Point(page.TopDivider.LeftX, page.TopDivider.Y));
            var topDividerEnd = pictureBox1.ToPictureBoxPoint(new Point(page.TopDivider.RightX, page.TopDivider.Y));
            var topDividerDragRectangle = pictureBox1.ToPictureBoxRectangle(page.TopDivider.DragRectangle);
            e.Graphics.DrawLine(dividerPen, topDividerStart, topDividerEnd);
            e.Graphics.FillRectangle(dividerBrush, topDividerDragRectangle);

            var bottomDividerStart = pictureBox1.ToPictureBoxPoint(new Point(page.BottomDivider.LeftX, page.BottomDivider.Y));
            var bottomDividerEnd = pictureBox1.ToPictureBoxPoint(new Point(page.BottomDivider.RightX, page.BottomDivider.Y));
            var bottomDividerDragRectangle = pictureBox1.ToPictureBoxRectangle(page.BottomDivider.DragRectangle);
            e.Graphics.DrawLine(dividerPen, bottomDividerStart, bottomDividerEnd);
            e.Graphics.FillRectangle(dividerBrush, bottomDividerDragRectangle);
        }

        private void AddWord(Rectangle pbRectangle, bool isCommentLink)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;
            var dialog = new WordTextDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var word = new OcrWord
            {
                TopLeftX = originalRect.X,
                TopLeftY = originalRect.Y,
                BottomRightX = originalRect.X + originalRect.Width,
                BottomRightY = originalRect.Y + originalRect.Height,
                Text = dialog.WordText,
                IsCommentLinkNumber = isCommentLink
            };

            var wordLine = page.Lines.FirstOrDefault(l => l.PageWideRectangle(page.Width).IntersectsWith(word.Rectangle));

            if (wordLine == null)
            {
                wordLine = new OcrLine {Words = new List<OcrWord>( )};
                page.Lines.Add(wordLine);
                wordLine.Features = OcrLineFeatures.Calculate(page, wordLine);
                page.Lines = page.Lines.OrderBy(l => l.TopLeftY).ToList();
                for (var i = 0; i < page.Lines.Count; i++) page.Lines[i].LineIndex = i;
            }
            
            wordLine.Words.Add(word);
            wordLine.Words = wordLine.Words.OrderBy(w => w.TopLeftX).ToList();
            wordLine.FitRectangleToWords();

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

        private void AddPageWideImageBlock(Rectangle pbRectangle)
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

        private void AddImageBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var imageBlock = new OcrImageBlock
            {
                TopLeftX = originalRect.X,
                TopLeftY = originalRect.Y,
                BottomRightX = originalRect.X + originalRect.Width,
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

        private void AddTitleBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);

            var titleLines = page.GetIntersectingLines(originalRect).ToList();

            var text = string.Join(" ", titleLines.Select(l => string.Join(" ", l.Words.Select(w => w.Text))));

            text = text.ToLower();

            text = text[0].ToString().ToUpper() + text.Substring(1);

            text = InitialsToUpper(text);

            _titleBlockDialog.TitleText = text;

            if (_titleBlockDialog.ShowDialog() != DialogResult.OK) return;

            var commentWords = titleLines.SelectMany(l => l.Words).Where(w => w.IsCommentLinkNumber).ToList();

            var titleBlock = new OcrTitleBlock
            {
                TopLeftX = originalRect.X,
                TopLeftY = originalRect.Y,
                BottomRightX = originalRect.X + originalRect.Size.Width,
                BottomRightY = originalRect.Y + originalRect.Size.Height,
                TitleLevel = _titleBlockDialog.TitleLevel,
                TitleText = _titleBlockDialog.TitleText,
                CommentLinks = commentWords.Select(w => 
                    new OcrWord
                    {
                        Text = w.Text,
                        IsCommentLinkNumber = true
                    }).ToList()
            };

            page.TitleBlocks ??= new List<OcrTitleBlock>();

            page.TitleBlocks.Add(titleBlock);

            foreach (var line in titleLines) line.Label = OcrLabel.Title;

            ocr_lb.Items[ocr_lb.SelectedIndex] = fileName;
        }

        private string InitialsToUpper(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var spaceSplit = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var words = new List<string>();

            foreach (var s in spaceSplit)
            {
                if (s.Length == 2 && s.EndsWith('.'))
                {
                    words.Add(s.ToUpper());
                }
                else
                {
                    words.Add(s);
                }
            }

            return string.Join(' ', words);
        }

        private void RemoveTitleBlock(Rectangle pbRectangle)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;
            var page = _ocrData.GetPage(fileName);
            if (page == null) return;

            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            page.TitleBlocks.RemoveAll(ib => ib.Rectangle.IntersectsWith(originalRect));

            pictureBox1.Refresh();
        }

        private void RegenerateFeaturesClick(object? sender, EventArgs e)
        {
            foreach (var page in _ocrData.Pages)
            {
                foreach (var line in page.Lines)
                {
                    line.Features = OcrLineFeatures.Calculate(page, line);
                }
            }

            MessageBox.Show("Features were regenerated", "Features", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void OcrLbOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                await RegeneratePage();
            }

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
                    
                    using var textBrush = new SolidBrush(Color.DarkViolet);
                    var font = new Font(Font.FontFamily, 12, FontStyle.Bold);

                    foreach (var line in page.Lines)
                    {
                        using var brush = GetBrush(line);
                        g.FillRectangle(brush, line.Rectangle);

                        using var pen = GetPen(line);
                        g.DrawRectangle(pen, line.Rectangle);

                        using var commentLinkBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 255));
                        using var commentLinkPen = new Pen(Color.Blue, 1);
                        using var linkTextBrush = new SolidBrush(Color.White);

                        foreach (var word in line.Words ?? new List<OcrWord>())
                        {
                            if (word.IsCommentLinkNumber)
                            {
                                var ellipseX = word.BottomRightX;
                                var ellipseY = word.TopLeftY - OcrSettings.WordCircleRadius * 2;
                                var ellipseSize = OcrSettings.WordCircleRadius * 2;
                                g.FillEllipse(commentLinkBrush, ellipseX, ellipseY, ellipseSize, ellipseSize);
                                g.DrawString(word.Text, font, linkTextBrush, ellipseX + 6, ellipseY + 4);
                                g.DrawRectangle(commentLinkPen, word.Rectangle);
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

            while (intersectingLines.Any())
            {
                var mergeRectangle = intersectingLines[0].PageWideRectangle(page.Width);
                var linesToMerge = intersectingLines.Where(l => l.Rectangle.IntersectsWith(mergeRectangle)).ToList();
                foreach (var line in linesToMerge)
                {
                    intersectingLines.Remove(line);
                }
                page.MergeLines(linesToMerge[0], linesToMerge.Skip(1).ToArray());
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

        private void BreakIntoWords(Rectangle pbRectangle)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var intersectingLines = page.Lines.Where(l => l.Rectangle.IntersectsWith(originalRect)).ToList();

            foreach (var intersectingLine in intersectingLines)
            {
                page.BreakIntoWords(intersectingLine);
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
        }

        private void SetWordText(Rectangle pbRectangle)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var intersectingLine = page.Lines.FirstOrDefault(l => l.Rectangle.IntersectsWith(originalRect));

            if (intersectingLine?.Words?.Any() != true) return;

            var intersectingWord = intersectingLine.Words.FirstOrDefault(w => w.Rectangle.IntersectsWith(originalRect));

            if (intersectingWord == null) return;

            var dialog = new WordTextDialog {WordText = intersectingWord.Text};

            if (dialog.ShowDialog() != DialogResult.OK) return;

            intersectingWord.Text = dialog.WordText;
            intersectingLine.DisplayText = true;

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
        }

        private void RemoveLine(Rectangle pbRectangle)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var originalRect = pictureBox1.ToOriginalRectangle(pbRectangle);
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var intersectingLine = page.Lines.FirstOrDefault(l => l.Rectangle.IntersectsWith(originalRect));

            if (intersectingLine == null) return;

            page.Lines.Remove(intersectingLine);

            page.ReindexLines();

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
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

        private void BreakByDistantClick(object sender, EventArgs e)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var page = _ocrData.GetPage(filename);
            if (page == null) return;
            var lines = page.Lines.ToArray();
            foreach (var line in lines)
            {
                page.BreakLineByDistantWord(line, 20);
            }

            ocr_lb.Items[ocr_lb.SelectedIndex] = filename;
        }

        private void UncoveredStartsClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var pages = _ocrData.Pages.OrderBy(p => p.ImageIndex)
                .Skip(dialog.MinImageIndex)
                .Take(1 + dialog.MaxImageIndex - dialog.MinImageIndex)
                .ToList();

            // find uncovered contours
            var uncoveredContours = new List<UncoveredContour>();
            foreach (var page in pages)
            {
                var imageFile = Directory.GetFiles(bookFolder_tb.Text)
                    .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == page.Filename);
                uncoveredContours.AddRange(CvUtil.GetUncoveredContours(imageFile, page));
            }

            // find line start contours
            var lineStartMatch = new LineStartMatch();
            var lineStartContours = uncoveredContours.Where(lineStartMatch.Match).ToList();
            if (lineStartContours.Any())
            {
                var lineStartDialog = new UncoveredContourDialog
                {
                    Text = "Mark uncovered line starts"
                };
                lineStartDialog.SetContours(lineStartContours);
                lineStartDialog.ShowDialog();
                lineStartContours = lineStartContours.Where(lc => !string.IsNullOrEmpty(lc.Word.Text)).ToList();
                foreach (var contour in lineStartContours)
                {
                    lineStartMatch.Apply(_ocrData, contour);
                }
            }
            else
            {
                MessageBox.Show("Uncovered line start contours were not found", "Uncovered line start contours",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RemoveLinksClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var pages = _ocrData.Pages.OrderBy(p => p.ImageIndex)
                .Skip(dialog.MinImageIndex)
                .Take(1 + dialog.MaxImageIndex - dialog.MinImageIndex)
                .ToList();

            foreach (var page in pages)
            {
                var lines = page.Lines.ToList();
                foreach (var line in lines)
                {
                    var words = line.Words?.ToList() ?? new List<OcrWord>();

                    var linkWords = words.Where(w => w.IsCommentLinkNumber).ToList();

                    if (!linkWords.Any()) continue;

                    if (linkWords.Count == words.Count)
                    {
                        page.Lines.Remove(line);
                        continue;
                    }

                    foreach (var linkWord in linkWords)
                    {
                        line.Words.Remove(linkWord);
                    }
                }
            }
        }

        private void TrainApplyModelClick(object sender, EventArgs e)
        {
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var page = _ocrData.GetPage(filename);
            if (page == null) return;

            var dialog = new ModelScopeDialog
            {
                ImageIndex = page.ImageIndex,
                TakeBefore = 20,
                TakeAfter = 1000
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var labeledLines = _ocrData.Pages.Where(dialog.BeforePageMatch)
                .SelectMany(p => p.GetExcludingLabels(OcrLabel.Comment, OcrLabel.Garbage, OcrLabel.Image, OcrLabel.Title))
                .Where(r => r.Label.HasValue && r.Features != null)
                .ToList();

            double[][] inputs = labeledLines.Select(l => l.Features.ToFeatureRow()).ToArray();
            int[] outputs = labeledLines.Select(l => (int)l.Label).ToArray();

            Accord.Math.Random.Generator.Seed = 1;
            var teacher = new RandomForestLearning
            {
                NumberOfTrees = 20
            };

            try
            {
                _model = teacher.Learn(inputs, outputs);

                var featuredLines = _ocrData.Pages.Where(dialog.AfterPageMatch)
                    .SelectMany(p => p.GetExcludingLabels(OcrLabel.Comment, OcrLabel.Garbage, OcrLabel.Image, OcrLabel.Title))
                    .Where(b => b.Features != null)
                    .ToList();

                var applyInputs = featuredLines.Select(l => l.Features.ToFeatureRow()).ToArray();
                var predicted = _model.Decide(applyInputs);

                for (var i = 0; i < featuredLines.Count; i++)
                {
                    featuredLines[i].Label = (OcrLabel)predicted[i];
                }

                MessageBox.Show("Model trained and applied", "Model", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void GenerateFb2Click(object sender, EventArgs e)
        {
            var templateDialog = new Fb2Dialog();

            if (templateDialog.ShowDialog() != DialogResult.OK) return;

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "FB2 files|*.fb2"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            var service = new Fb2Service();

            var template = File.ReadAllText("fb2template.xml");
            var templateData = templateDialog.TemplateData;
            template = template
                .Replace("[book-id]", templateData.BookId)
                .Replace("[book-title]", templateData.BookTitle)
                .Replace("[book-annotation]", templateData.BookAnnotation)
                .Replace("[genre]", templateData.BookGenre)
                .Replace("[author-first-name]", templateData.BookAuthorFirstName)
                .Replace("[author-last-name]", templateData.BookAuthorLastName)
                .Replace("[author-middle-name]", templateData.BookAuthorMiddleName)
                .Replace("[doc-author-first-name]", templateData.DocAuthorFirstName)
                .Replace("[doc-author-last-name]", templateData.DocAuthorLastName)
                .Replace("[doc-id]", templateData.DocId)
                .Replace("[doc-version]", templateData.DocVersion);

            try
            {
                service.GenerateFb2File(bookFolder_tb.Text, saveFileDialog.FileName, template);

                if (MessageBox.Show("FB2 file was generated fine. Do you want to open the file?", "FB2",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) 
                    return;

                using var process = new Process
                {
                    StartInfo = {FileName = "explorer", Arguments = $"\"{saveFileDialog.FileName}\""}
                };
                process.Start();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
