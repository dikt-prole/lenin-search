using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Detectors;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;
using BookProject.WinForms.Controls.BlockDetails;
using BookProject.WinForms.DragActivities;
using BookProject.WinForms.PageActions;

namespace BookProject.WinForms
{
    /*
     * todo:
     * 2. новый детектор для comment link - через top margin (приблизительно половина сверзу - половина снизу)
     */
    public partial class BookProjectForm : Form
    {
        private readonly BookViewModel _bookVm;

        private IImageRenderer _imageRenderer;

        private IDragActivity _dragActivity;

        private BookProjectSettings _settings;

        private readonly PreviewKeyDownPageActionFactory
            _previewKeyDownPageActionFactory = new PreviewKeyDownPageActionFactory();

        private readonly IOcrUtility _ocrUtility = new YandexVisionOcrUtility();

        private readonly CommentLinkBlockDetailsControl _commentLinkBlockDetailsControl;

        private readonly TitleBlockDetailsControl _titleBlockDetailsControl;

        public BookProjectForm()
        {
            InitializeComponent();

            _bookVm = new BookViewModel();
            _bookVm.SelectedBlockChanged += EditBlockSelectionChanged;
            _bookVm.BlockAdded += BlockAdded;
            _bookVm.BlockRemoved += BlockRemoved;
            _bookVm.BlockModified += BlockModified;

            pictureBox1.Paint += PictureBox1OnPaint;
            pictureBox1.MouseDown += PictureBox1OnMouseDown;
            pictureBox1.MouseUp += PictureBox1OnMouseUp;
            pictureBox1.MouseMove += PictureBox1OnMouseMove;

            saveBook_btn.Click += OnSaveBookClick;

            openBookFolder_btn.Click += OnOpenBookFolderClick;
            generateLines_btn.Click += GenerateLinesClick;

            _imageRenderer = new PageStateRenderer(_bookVm);

            _settings = BookProjectSettings.Load();

            // image detection
            detectImageControl1.SetSettings(_settings.ImageDetection);
            detectImageControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectImageImageRenderer(detectImageControl1.GetSettings());
                pictureBox1.Refresh();
            };
            detectImageControl1.Detect += OnDetectImageClick;
            detectImageControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();
            detectImageControl1.Save += (sender, args) =>
            {
                _settings.ImageDetection = detectImageControl1.GetSettings();
                _settings.Save();
                MessageBox.Show("Saved!", "Image detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // title detection
            detectTitleControl1.SetSettings(_settings.TitleDetection);
            detectTitleControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectTitleImageRenderer(detectTitleControl1.GetSettings());
                pictureBox1.Refresh();
            };
            detectTitleControl1.Detect += OnDetectTitleClick;
            detectTitleControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();
            detectTitleControl1.Save += (sender, args) =>
            {
                _settings.TitleDetection = detectTitleControl1.GetSettings();
                _settings.Save();
                MessageBox.Show("Saved!", "Title detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // comment link detection
            detectCommentLinkNumberControl1.SetSettings(_settings.CommentLinkDetection);
            detectCommentLinkNumberControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectCommentLinkNumberImageRenderer(detectCommentLinkNumberControl1.GetSettings(), new YandexVisionOcrUtility());
                pictureBox1.Refresh();
            };
            detectCommentLinkNumberControl1.Detect += OnDetectCommentLinkClick;
            detectCommentLinkNumberControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();
            detectCommentLinkNumberControl1.Save += (sender, args) =>
            {
                _settings.CommentLinkDetection = detectCommentLinkNumberControl1.GetSettings();
                _settings.Save();
                MessageBox.Show("Saved!", "Comment link detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // garbage detection
            detectGarbageControl1.SetSettings(_settings.GarbageDetection);
            detectGarbageControl1.TestStart += (sender, args) =>
            {
                _imageRenderer = new TestDetectGarbageImageRenderer(detectGarbageControl1.GetSettings());
                pictureBox1.Refresh();
            };
            detectGarbageControl1.Detect += OnDetectGarbageClick;
            detectGarbageControl1.TestEnd += (sender, args) => SetPageStateImageRenderer();
            detectGarbageControl1.Save += (sender, args) =>
            {
                _settings.GarbageDetection = detectGarbageControl1.GetSettings();
                _settings.Save();
                MessageBox.Show("Saved!", "Garbage detection settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            pictureBox1.PreviewKeyDown += PictureBox1OnPreviewKeyDown;

            _commentLinkBlockDetailsControl = new CommentLinkBlockDetailsControl();
            _titleBlockDetailsControl = new TitleBlockDetailsControl { OcrUtility = _ocrUtility };
        }

        private void BlockModified(object sender, Block e)
        {
            if (_bookVm.GetBlockPage(e) == _bookVm.CurrentPage)
            {
                pictureBox1.Refresh();
            }
        }

        private void BlockRemoved(object sender, Block e)
        {
            if (_bookVm.GetBlockPage(e) == _bookVm.CurrentPage)
            {
                pictureBox1.Refresh();
            }
        }

        private void BlockAdded(object sender, Block e)
        {
            if (_bookVm.GetBlockPage(e) == _bookVm.CurrentPage)
            {
                pictureBox1.Refresh();
            }
        }

        private void EditBlockSelectionChanged(object sender, Block e)
        {
            blockDetails_panel.Controls.Clear();

            if (e is CommentLinkBlock commentLinkBlock)
            {
                _commentLinkBlockDetailsControl.SetBlock(_bookVm, commentLinkBlock);
                blockDetails_panel.Controls.Add(_commentLinkBlockDetailsControl);
                _commentLinkBlockDetailsControl.Dock = DockStyle.Fill;
            }

            if (e is TitleBlock titleBlock)
            {
                _titleBlockDetailsControl.SetBlock(_bookVm, titleBlock);
                blockDetails_panel.Controls.Add(_titleBlockDetailsControl);
                _titleBlockDetailsControl.Dock = DockStyle.Fill;
            }

            if (_bookVm.GetBlockPage(e) == _bookVm.CurrentPage)
            {
                pictureBox1.Refresh();
            }
        }

        private void PictureBox1OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var pageAction = _previewKeyDownPageActionFactory.Construct(e);
            if (pageAction != null)
            {
                e.IsInputKey = true;
                pageAction.Execute(_bookVm);
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
                var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle)
                    .Concat(page.TitleBlocks.Select(b => b.Rectangle))
                    .Concat(page.CommentLinkBlocks.Select(b => b.Rectangle))
                    .ToArray();
                var garbageRects = new GarbageDetector()
                    .Detect(ImageUtility.Load(imageFile), detectGarbageControl1.GetSettings(), excludeRects, null);
                _bookVm.SetPageBlocks(page, garbageRects.Select(GarbageBlock.FromRectangle));
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

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
                var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle)
                    .ToArray();
                var commentLinkRects = new CommentLinkDetector(new YandexVisionOcrUtility())
                    .Detect(ImageUtility.Load(imageFile), detectCommentLinkNumberControl1.GetSettings(), excludeRects, null);
                _bookVm.SetPageBlocks(page, commentLinkRects.Select(GarbageBlock.FromRectangle));
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

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
                var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var imageRects = new ImageDetector().Detect(ImageUtility.Load(imageFile), detectImageControl1.GetSettings(), null, null);
                _bookVm.SetPageBlocks(page, imageRects.Select(ImageBlock.FromRectangle));
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

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
                var page = _bookVm.Book.GetPage(Path.GetFileNameWithoutExtension(imageFile));
                var excludeRects = page.ImageBlocks.Select(b => b.Rectangle).ToArray();
                var titleRects = new TitleDetector().Detect(ImageUtility.Load(imageFile), detectTitleControl1.GetSettings(), excludeRects, null);
                _bookVm.SetPageBlocks(page, titleRects.Select(TitleBlock.FromRectangle));
                progressBar1.Value = i + 1;
                Application.DoEvents();
            }

            MessageBox.Show("Completed!", "Detect Titles", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
        }

        private void SetPageStateImageRenderer()
        {
            _imageRenderer = new PageStateRenderer(_bookVm);
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

        private void OnOpenBookFolderClick(object? sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() != DialogResult.OK) return;

            _bookVm.Book = Book.Load(dialog.SelectedPath);

            bookFolder_tb.Text = dialog.SelectedPath;

            blockListControl1.SetBookVm(_bookVm);
        }

        private void OnSaveBookClick(object? sender, EventArgs e)
        {
            _bookVm.Book.Save(bookFolder_tb.Text);

            MessageBox.Show("Ocr data saved", "Ocr data", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PictureBox1OnMouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Focus();

            if (pictureBox1.Image == null) return;

            if (e.Button == MouseButtons.Right)
            {
                _bookVm.OriginalSelectionStartPoint = pictureBox1.ToOriginalPoint(e.Location);
                _bookVm.PbSelectionStartPoint = e.Location;
            }

            if (e.Button == MouseButtons.Left)
            {
                var originalPoint = pictureBox1.ToOriginalPoint(e.Location);
                var editBlock = _bookVm.SelectedBlock;
                if (editBlock != null)
                {
                    var dragPointLabel = DragPointLabelResolver.GetDragLabelAtPoint(editBlock, pictureBox1, e.Location);
                    if (dragPointLabel.HasValue)
                    {
                        _dragActivity = DragActivityFactory.ConstructDragActivity(_bookVm, editBlock, dragPointLabel.Value);
                        return;
                    }
                }

                _dragActivity = null;
                var blockAtCursor = _bookVm.CurrentPage.GetAllBlocks().FirstOrDefault(b => b.Rectangle.Contains(originalPoint));
                _bookVm.SetBlockSelected(blockAtCursor);
            }
        }

        private void PictureBox1OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_bookVm.CurrentPage == null) return;

            _dragActivity = null;

            if (e.Button == MouseButtons.Right && _bookVm.OriginalSelectionStartPoint.HasValue)
            {
                var originalLocation = pictureBox1.ToOriginalPoint(e.Location);

                var xs = new List<int>
                {
                    _bookVm.OriginalSelectionStartPoint.Value.X,
                    originalLocation.X
                }
                    .OrderBy(i => i).ToList();

                var ys = new List<int>
                {
                    _bookVm.OriginalSelectionStartPoint.Value.Y,
                    originalLocation.Y
                }
                    .OrderBy(i => i).ToList();

                var originalRect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                var menu = GetPageMenu(_bookVm.CurrentPage, originalRect);
                menu.Show(pictureBox1, e.Location);
                _bookVm.OriginalSelectionStartPoint = null;
                _bookVm.PbSelectionStartPoint = null;
            }
        }

        private ContextMenuStrip GetPageMenu(Page page, Rectangle originalRect)
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Add Image Block", null,
                (o, a) => _bookVm.AddBlock(ImageBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Title Block", null,
                (o, a) => _bookVm.AddBlock(TitleBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Comment Link Block", null, 
                (o, a) => _bookVm.AddBlock(CommentLinkBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Garbage Block", null, 
                (o, a) => _bookVm.AddBlock(GarbageBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Recognize Text", null, async (o, a) =>
            {
                using var ocrBitmap = ImageUtility.Crop(_bookVm.OriginalPageBitmap, originalRect);
                using var ocrStream = new MemoryStream();
                ocrBitmap.Save(ocrStream, ImageFormat.Jpeg);
                var ocrPage = await _ocrUtility.GetPageAsync(ocrStream.ToArray());
                Clipboard.SetText(ocrPage.GetText());
                MessageBox.Show("Text copied to clipboard", "Recognize Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

            return menu;
        }

        private void PictureBox1OnMouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image == null) return;

            _bookVm.OriginalMouseAt = pictureBox1.ToOriginalPoint(e.Location);
            _bookVm.PictureBoxMouseAt = e.Location;

            if (e.Button == MouseButtons.Right && _bookVm.OriginalSelectionStartPoint.HasValue)
            {
                pictureBox1.Refresh();
            }

            if (_dragActivity != null)
            {
                _dragActivity.Perform(pictureBox1, e);
            }
        }

        private void PictureBox1OnPaint(object sender, PaintEventArgs e)
        {
            if (_bookVm.CurrentPage == null) return;

            _imageRenderer.Render(_bookVm.OriginalPageBitmap, e.Graphics);
        }

        private void GenerateFb2Click(object sender, EventArgs e)
        {
            /*
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
            */
        }
    }
}
