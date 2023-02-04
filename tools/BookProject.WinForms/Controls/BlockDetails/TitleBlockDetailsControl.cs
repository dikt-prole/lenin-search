using System;
using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.Controls.BlockDetails
{
    public partial class TitleBlockDetailsControl : UserControl
    {
        public event EventHandler<TitleBlock> RecognizeText; 

        private TitleBlock _titleBlock;
        public TitleBlockDetailsControl()
        {
            InitializeComponent();
            text_tb.KeyUp += (sender, args) =>
            {
                if (_titleBlock != null)
                {
                    _titleBlock.Text = text_tb.Text;
                }
            };

            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 10;
            numericUpDown1.Value = 0;
            numericUpDown1.ValueChanged += (sender, args) =>
            {
                _titleBlock.Level = (int)numericUpDown1.Value;
            };

            recognizeText_btn.Click += (sender, args) => RecognizeText?.Invoke(this, _titleBlock);

            toLower_btn.Click += (sender, args) =>
            {
                _titleBlock.Text = _titleBlock.Text.ToLower();
                text_tb.Text = _titleBlock.Text;
            };
            toUpper_btn.Click += (sender, args) =>
            {
                _titleBlock.Text = _titleBlock.Text.ToUpper();
                text_tb.Text = _titleBlock.Text;
            };
        }

        public void SetBlock(TitleBlock titleBlock)
        {
            _titleBlock = titleBlock;
            text_tb.Text = _titleBlock.Text;
        }
    }
}
