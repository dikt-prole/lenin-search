using System;
using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class TitleBlockDetailsControl : UserControl
    {
        public event EventHandler<TitleBlock> RecognizeText;

        public event EventHandler<TitleBlock> BlockChanged; 

        private TitleBlock _titleBlock;
        public TitleBlockDetailsControl()
        {
            InitializeComponent();
            text_tb.KeyUp += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _titleBlock.Text = text_tb.Text;
                    BlockChanged?.Invoke(this, _titleBlock);
                }
            };

            level_nud.Minimum = 0;
            level_nud.Maximum = 10;
            level_nud.Value = 0;
            level_nud.ValueChanged += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _titleBlock.Level = (int)level_nud.Value;
                    BlockChanged?.Invoke(this, _titleBlock);
                }
            };

            recognizeText_btn.Click += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    RecognizeText?.Invoke(this, _titleBlock);
                    text_tb.Text = _titleBlock.Text;
                    BlockChanged?.Invoke(this, _titleBlock);
                }
            };

            toLower_btn.Click += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _titleBlock.Text = _titleBlock.Text?.ToLower();
                    text_tb.Text = _titleBlock.Text;
                    BlockChanged?.Invoke(this, _titleBlock);
                }
            };
            toUpper_btn.Click += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _titleBlock.Text = _titleBlock.Text?.ToUpper();
                    text_tb.Text = _titleBlock.Text;
                    BlockChanged?.Invoke(this, _titleBlock);
                }
            };
        }

        public void SetBlock(TitleBlock titleBlock)
        {
            _titleBlock = titleBlock;
            text_tb.Text = _titleBlock.Text;
            level_nud.Value = _titleBlock.Level;
        }
    }
}
