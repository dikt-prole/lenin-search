using System.Drawing;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.AddBlock
{
    public class AddBlockPageAction<TBlock> : IPageAction where TBlock : Block
    {
        private readonly int _width;
        private readonly int _height;

        public AddBlockPageAction(int width = 100, int height = 100)
        {
            _width = width;
            _height = height;
        }

        public void Execute(BookViewModel bookVm)
        {
            var blockRect = new Rectangle(
                (bookVm.CurrentPage.Width - _width) / 2, 
                (bookVm.CurrentPage.Height - _height) / 2, 
                _width, 
                _height);

            var blockType = typeof(TBlock);

            if (blockType == typeof(ImageBlock))
            {
                bookVm.AddBlock(ImageBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }

            if (blockType == typeof(TitleBlock))
            {
                bookVm.AddBlock(TitleBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }

            if (blockType == typeof(GarbageBlock))
            {
                bookVm.AddBlock(GarbageBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }

            if (blockType == typeof(CommentLinkBlock))
            {
                bookVm.AddBlock(CommentLinkBlock.FromRectangle(blockRect), bookVm.CurrentPage);
            }
        }
    }
}