using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Detectors;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Book;
using BookProject.Core.Utilities;
using BookProject.WinForms.MouseMoveActivities;
using BookProject.WinForms.Service;

namespace BookProject.WinForms
{
    public partial class BookProjectForm : Form
    {
        private Book _book = null;

        private readonly PageState _pageState = new PageState();

        private IImageRenderer _imageRenderer;

        private IMouseMoveActivity _mouseMoveActivity;

        public BookProjectForm()
        {
            InitializeComponent();

            ocr_lb.SelectedIndexChanged += OcrLbOnSelectedIndexChanged;

            pictureBox1.Paint += PictureBox1OnPaint;
            pictureBox1.MouseDown += PictureBox1OnMouseDown;
            pictureBox1.MouseUp += PictureBox1OnMouseUp;
            pictureBox1.MouseMove += PictureBox1OnMouseMove;

            saveBook_btn.Click += OnSaveBookClick;

            openBookFolder_btn.Click += OpenBookFolderClick;
            generateLines_btn.Click += GenerateLinesClick;

            _imageRenderer = new PageStateRenderer(_pageState);

            detectTitleControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectTitleImageRenderer(detectTitleControl1.GetSettings());
                pictureBox1.Refresh();
            };
            detectTitleControl1.Detect += OnDetectTitleClick;

            detectImageControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectImageImageRenderer(detectImageControl1.GetSettings());
                pictureBox1.Refresh();
            };
            detectImageControl1.Detect += OnDetectImageClick;

            detectCommentLinkNumberControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectCommentLinkNumberImageRenderer(detectCommentLinkNumberControl1.GetSettings(), new YandexVisionOcrUtility());
                pictureBox1.Refresh();
            };
            detectCommentLinkNumberControl1.Detect += OnDetectCommentLinkClick;

            detectGarbageControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectGarbageImageRenderer(detectGarbageControl1.GetSettings());
                pictureBox1.Refresh();
            };
            detectGarbageControl1.Detect += OnDetectGarbageClick;

            detectTitleControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();

            detectImageControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();

            detectCommentLinkNumberControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();

            detectGarbageControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();

            KeyDown += PictureBox1OnKeyDown;
        }

        private void PictureBox1OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var editedBlock = _pageState.Page.GetEditBlock();
                if (editedBlock != null)
                {
                    _pageState.Page.RemoveBlock(editedBlock);
                }
            }
        }

        private void OnDetectGarbageClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = dialog.GetMatchingImages(bookFolder_tb.Text);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Length;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Length; i++)
            {
                var imageFile = imageFiles[i];
                var page = _book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle)
                    .Concat(page.TitleBlocks.Select(b => b.Rectangle))
                    .Concat(page.CommentLinkBlocks.Select(b => b.Rectangle))
                    .ToArray();
                var garbageRects = new GarbageDetector()
                    .Detect(imageFile, detectGarbageControl1.GetSettings(), excludeRects, null);
                page.GarbageBlocks = garbageRects.Select(GarbageBlock.FromRectangle).ToList();
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            pictureBox1.Refresh();
            MessageBox.Show("Completed!", "Detect Garbage", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void OnDetectCommentLinkClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = dialog.GetMatchingImages(bookFolder_tb.Text);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Length;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Length; i++)
            {
                var imageFile = imageFiles[i];
                var page = _book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle)
                    .Concat(page.TitleBlocks.Select(b => b.Rectangle))
                    .ToArray();
                var commentLinkRects = new CommentLinkDetector(new YandexVisionOcrUtility())
                    .Detect(imageFile, detectCommentLinkNumberControl1.GetSettings(), excludeRects, null);
                page.CommentLinkBlocks = commentLinkRects.Select(CommentLinkBlock.FromRectangle).ToList();
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            pictureBox1.Refresh();
            MessageBox.Show("Completed!", "Detect Comment Links", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void OnDetectImageClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = dialog.GetMatchingImages(bookFolder_tb.Text);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Length;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Length; i++)
            {
                var imageFile = imageFiles[i];
                var page = _book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var imageRects = new ImageDetector().Detect(imageFile, detectImageControl1.GetSettings(), null, null);
                page.ImageBlocks = imageRects.Select(ImageBlock.FromRectangle).ToList();
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            pictureBox1.Refresh();
            MessageBox.Show("Completed!", "Detect Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void OnDetectTitleClick(object sender, EventArgs e)
        {
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = dialog.GetMatchingImages(bookFolder_tb.Text);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Length;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Length; i++)
            {
                var imageFile = imageFiles[i];
                var page = _book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle).ToArray();
                var titleRects = new TitleDetector().Detect(imageFile, detectTitleControl1.GetSettings(), excludeRects, null);
                page.TitleBlocks = titleRects.Select(TitleBlock.FromRectangle).ToList();
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            pictureBox1.Refresh();
            MessageBox.Show("Completed!", "Detect Titles", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void SetPageStateImageRenderer()
        {
            _imageRenderer = new PageStateRenderer(_pageState);
            pictureBox1.Refresh();
        }

        private async void GenerateLinesClick(object? sender, EventArgs e)
        {
            /*
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var imageFiles = GetImageFiles(bookFolder_tb.Text)
                .Where(f => dialog.ImageMatch(int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray()))))
                .ToList();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = imageFiles.Count;
            progressBar1.Value = 0;

            for (var i = 0; i < imageFiles.Count; i++)
            {
                var imageFile = imageFiles[i];
                progressBar1.Value = i + 1;
            }

            MessageBox.Show("Ocr completed", "Ocr", MessageBoxButtons.OK, MessageBoxIcon.Information);
            */
        }

        private void UncoveredLinksClick(object sender, EventArgs e)
        {
            /*
            var dialog = new ImageScopeDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            var pages = _bookProjectData.Pages.OrderBy(p => p.ImageIndex)
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
                    linkNumberMatch.Apply(_bookProjectData, contour);
                }
            }
            else
            {
                MessageBox.Show("Uncovered link number contours were not found", "Uncovered link number contours",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            */
        }

        private void OpenBookFolderClick(object? sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _book = Book.Load(dialog.SelectedPath);

            bookFolder_tb.Text = dialog.SelectedPath;

            ocr_lb.Items.Clear();

            var fileNames = Directory.GetFiles(bookFolder_tb.Text, "*.jpg")
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(f => int.Parse(new string(Path.GetFileNameWithoutExtension(f).Where(char.IsNumber).ToArray())))
                .ToList();

            foreach (var fileName in fileNames)
            {
                ocr_lb.Items.Add(fileName);
            }
        }

        private void OnSaveBookClick(object? sender, EventArgs e)
        {
            _book.Save(bookFolder_tb.Text);

            MessageBox.Show("Ocr data saved", "Ocr data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PictureBox1OnMouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;
            var filename = ocr_lb.SelectedItem as string;
            if (filename == null) return;
            var page = _book.GetPage(filename);
            if (page == null) return;

            if (e.Button == MouseButtons.Right)
            {
                _pageState.SelectionStartPoint = pictureBox1.ToOriginalPoint(e.Location);
            }

            if (e.Button == MouseButtons.Left)
            {
                var originalPoint = pictureBox1.ToOriginalPoint(e.Location);
                var editBlock = page.GetEditBlock();
                if (editBlock?.LeftDragRectangle.Contains(originalPoint) == true)
                {
                    _mouseMoveActivity = new DragBlockLeftMouseMoveActivity(editBlock);
                }
                else if (editBlock?.RightDragRectangle.Contains(originalPoint) == true)
                {
                    _mouseMoveActivity = new DragBlockRightMouseMoveActivity(editBlock);
                }
                else if (editBlock?.TopDragRectangle.Contains(originalPoint) == true)
                {
                    _mouseMoveActivity = new DragBlockTopMouseMoveActivity(editBlock);
                }
                else if (editBlock?.BottomDragRectangle.Contains(originalPoint) == true)
                {
                    _mouseMoveActivity = new DragBlockBottomMouseMoveActivity(editBlock);
                }
                else
                {
                    _mouseMoveActivity = null;
                    var blockAtCursor = page.GetAllBlocks().FirstOrDefault(b => b.Rectangle.Contains(originalPoint));
                    if (blockAtCursor != null)
                    {
                        page.SetEditBlock(blockAtCursor);
                        pictureBox1.Refresh();
                    }
                }
            }
        }

        private void PictureBox1OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_pageState.Page == null) return;

            _mouseMoveActivity = null;

            if (e.Button == MouseButtons.Right && _pageState.SelectionStartPoint.HasValue)
            {
                var originalLocation = pictureBox1.ToOriginalPoint(e.Location);

                var xs = new List<int>
                {
                    _pageState.SelectionStartPoint.Value.X,
                    originalLocation.X
                }
                    .OrderBy(i => i).ToList();

                var ys = new List<int>
                {
                    _pageState.SelectionStartPoint.Value.Y,
                    originalLocation.Y
                }
                    .OrderBy(i => i).ToList();

                var originalRect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                var menu = GetPageMenu(_pageState.Page, originalRect);
                menu.Show(pictureBox1, e.Location);
                _pageState.SelectionStartPoint = null;
            }
        }

        private ContextMenuStrip GetPageMenu(Page page, Rectangle originalRect)
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Add Image Block", null, (o, a) =>
            {
                page.ImageBlocks.Add(ImageBlock.FromRectangle(originalRect));
                pictureBox1.Refresh();
            });

            menu.Items.Add("Add Title Block", null, (o, a) =>
            {
                page.TitleBlocks.Add(TitleBlock.FromRectangle(originalRect));
                pictureBox1.Refresh();
            });

            menu.Items.Add("Add Comment Link Block", null, (o, a) =>
            {
                page.CommentLinkBlocks.Add(CommentLinkBlock.FromRectangle(originalRect));
                pictureBox1.Refresh();
            });

            menu.Items.Add("Add Garbage Block", null, (o, a) =>
            {
                page.GarbageBlocks.Add(GarbageBlock.FromRectangle(originalRect));
                pictureBox1.Refresh();
            });

            return menu;
        }

        private void PictureBox1OnMouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;

            _pageState.MouseAt = pictureBox1.ToOriginalPoint(e.Location);

            if (e.Button == MouseButtons.Right && _pageState.SelectionStartPoint.HasValue)
            {
                pictureBox1.Refresh();
            }

            if (_mouseMoveActivity != null)
            {
                _mouseMoveActivity.Perform(pictureBox1, e);
                pictureBox1.Refresh();
            }
        }

        private void PictureBox1OnPaint(object sender, PaintEventArgs e)
        {
            var filename = ocr_lb.SelectedItem as string;

            if (string.IsNullOrEmpty(filename)) return;

            var imageFile = Directory.GetFiles(bookFolder_tb.Text)
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == filename);

            using var imageStream = new MemoryStream();

            _imageRenderer.RenderBmp(imageFile, imageStream, pictureBox1.Width, pictureBox1.Height);

            var canvasBitmap = new Bitmap(imageStream);

            e.Graphics.DrawImage(canvasBitmap, 0, 0);
        }

        private void OcrLbOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            var fileName = ocr_lb.SelectedItem as string;
            if (fileName == null) return;

            var imageFile = Directory.GetFiles(bookFolder_tb.Text).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == fileName);
            if (imageFile == null)
            {
                MessageBox.Show("Image file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _pageState.Page = _book.GetPage(fileName);
            pictureBox1.Image = ImageUtility.Load(imageFile);
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
                    StartInfo = { FileName = "explorer", Arguments = $"\"{saveFileDialog.FileName}\"" }
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
