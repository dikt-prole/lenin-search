using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Utilities;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class TitleBlockDetailsControl : UserControl
    {
        private TitleBlock _titleBlock;
        private BookViewModel _bookVm;
        public TitleBlockDetailsControl()
        {
            InitializeComponent();
            text_tb.KeyUp += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _bookVm.ModifyBlock(this, _titleBlock, b =>
                    {
                        b.Text = text_tb.Text;
                    });
                }
            };

            level_nud.Minimum = 0;
            level_nud.Maximum = 10;
            level_nud.Value = 0;
            level_nud.ValueChanged += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _bookVm.ModifyBlock(this, _titleBlock, b =>
                    {
                        _titleBlock.Level = (int)level_nud.Value;
                    });
                }
            };

            recognizeText_btn.Click += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _bookVm.ModifyBlock(this, _titleBlock, b =>
                    {
                        using var ocrBitmap = ImageUtility.Crop(_bookVm.OriginalPageBitmap, _titleBlock.Rectangle);
                        using var ocrStream = new MemoryStream();
                        ocrBitmap.Save(ocrStream, ImageFormat.Jpeg);
                        var ocrPage = Task.Run(() => _bookVm.OcrUtility.GetPageAsync(ocrStream.ToArray())).Result;
                        b.Text = ocrPage.GetText();
                        text_tb.Text = b.Text;
                    });
                }
            };

            toLower_btn.Click += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _bookVm.ModifyBlock(this, _titleBlock, b =>
                    {
                        b.Text = b.Text?.ToLower();
                        text_tb.Text = b.Text;
                    });
                }
            };
            toUpper_btn.Click += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _bookVm.ModifyBlock(this, _titleBlock, b =>
                    {
                        b.Text = b.Text?.ToUpper();
                        text_tb.Text = b.Text;
                    });
                }
            };
        }

        public void SetBlock(BookViewModel bookVm, TitleBlock titleBlock)
        {
            _bookVm = bookVm;
            _titleBlock = titleBlock;
            text_tb.Text = _titleBlock.Text;
            level_nud.Value = _titleBlock.Level;
        }
    }
}
