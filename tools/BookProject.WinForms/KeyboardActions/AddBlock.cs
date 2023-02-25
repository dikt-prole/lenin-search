using System.Drawing;
using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions
{
    public class AddBlock<TBlock> : IKeyboardAction where TBlock : Block
    {
        private readonly int _width;
        private readonly int _height;

        public AddBlock(int width = 100, int height = 100)
        {
            _width = width;
            _height = height;
        }

        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            if (!args.Control) return;

            var blockRect = new Rectangle(
                (bookVm.CurrentPage.Rectangle.Width - _width) / 2,
                (bookVm.CurrentPage.Rectangle.Height - _height) / 2,
                _width,
                _height);

            var blockType = typeof(TBlock);

            if (blockType == typeof(ImageBlock))
            {
                bookVm.AddBlock(sender, ImageBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }

            if (blockType == typeof(TitleBlock))
            {
                bookVm.AddBlock(sender, TitleBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }

            if (blockType == typeof(GarbageBlock))
            {
                bookVm.AddBlock(sender, GarbageBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }

            if (blockType == typeof(CommentLinkBlock))
            {
                bookVm.AddBlock(sender, CommentLinkBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }
        }
    }
}