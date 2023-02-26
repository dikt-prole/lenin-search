using System;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Settings;

namespace BookProject.WinForms
{
    /*
     * todo:
     * 1. сделать общий контрол для деталей и биндить его
     * 2. инициализация bookVm
     * 3. новый детектор для comment link - через top margin (приблизительно половина сверзу - половина снизу)
     */
    public partial class BookProjectForm : Form
    {
        private BookViewModel _bookVm;

        public BookProjectForm()
        {
            InitializeComponent();
            saveBook_btn.Click += OnSaveBookClick;
            openBookFolder_btn.Click += OnOpenBookFolderClick;
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

            _bookVm = BookViewModel.Initialize(dialog.SelectedPath);
            bookFolder_tb.Text = dialog.SelectedPath;

            detectImageControl1.Bind(_bookVm);
            detectTitleControl1.Bind(_bookVm);
            detectGarbageControl1.Bind(_bookVm);
            detectCommentLinkNumberControl1.Bind(_bookVm);
            detectLineControl1.Bind(_bookVm);
            blockDetailsControl1.Bind(_bookVm);
            blockListControl1.Bind(_bookVm);
            pageControl1.Bind(_bookVm);
        }

        private void OnSaveBookClick(object? sender, EventArgs e)
        {
            _bookVm.Book.Save(bookFolder_tb.Text);
            MessageBox.Show("Ocr data saved", "Ocr data", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
