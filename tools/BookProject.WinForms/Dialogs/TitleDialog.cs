using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Utilities;
using Task = System.Threading.Tasks.Task;

namespace BookProject.WinForms.Dialogs
{
    public partial class TitleDialog : Form
    {
        private BookViewModel _bookVm;
        private Rectangle _titleRectangle;

        public TitleDialog()
        {
            InitializeComponent();
            Icon = new Icon("book_project_icon.ico");
            ShowInTaskbar = false;
            ok_btn.Click += OkBtnOnCLick;
            text_tb.KeyDown += OnKeyDown;
            Shown += OnShown;

            level_nud.Minimum = 0;
            level_nud.Maximum = 10;
            level_nud.Value = 0;
            level_nud.KeyDown += OnKeyDown;

            upper_btn.Click += UpperBtnOnClick;
            lower_btn.Click += LowerBtnOnClick;
            ocr_btn.Click += OcrBtnOnClick;
        }

        private void OcrBtnOnClick(object sender, EventArgs e)
        {
            if (_bookVm == null) return;

            var bitmap = ImageUtility.Crop(_bookVm.OriginalPageBitmap, _titleRectangle);
            var bitmapBytes = ImageUtility.GetJpegBytes(bitmap);
            var ocrPage = Task.Run(() => _bookVm.OcrUtility.GetPageAsync(bitmapBytes)).Result;
            text_tb.Text = ocrPage.GetText();
            text_tb.SelectAll();
            text_tb.Focus();
        }

        public TitleDialog Init(TitleBlock titleBlock, BookViewModel bookVm)
        {
            _bookVm = bookVm;
            _titleRectangle = titleBlock.Rectangle;
            level_nud.Value = titleBlock.Level;
            text_tb.Text = titleBlock.Text;
            return this;
        }

        public void Apply(TitleBlock titleBlock)
        {
            titleBlock.Text = text_tb.Text;
            titleBlock.Level = (int)level_nud.Value;
        }

        private void LowerBtnOnClick(object sender, EventArgs e)
        {
            text_tb.Text = text_tb.Text?.ToLower();
        }

        private void UpperBtnOnClick(object sender, EventArgs e)
        {
            text_tb.Text = text_tb.Text?.ToUpper();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                DialogResult = DialogResult.OK;
                Close();
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void OnShown(object sender, EventArgs e)
        {
            text_tb.SelectAll();
            text_tb.Focus();
        }

        private void OkBtnOnCLick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
