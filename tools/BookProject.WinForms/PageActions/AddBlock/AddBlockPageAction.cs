using System.Drawing;
using BookProject.Core.Models.Book;

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

        public void Execute(Page page)
        {
            var blockRect = new Rectangle((page.Width - _width) / 2, (page.Height - _height) / 2, _width, _height);

            var blockType = typeof(TBlock);

            if (blockType == typeof(ImageBlock))
            {
                var imageBlock = ImageBlock.FromRectangle(blockRect);
                page.ImageBlocks.Add(imageBlock);
                page.SetEditBlock(imageBlock);
            }

            if (blockType == typeof(TitleBlock))
            {
                var titleBlock = TitleBlock.FromRectangle(blockRect);
                page.TitleBlocks.Add(titleBlock);
                page.SetEditBlock(titleBlock);
            }

            if (blockType == typeof(GarbageBlock))
            {
                var garbageBlock = GarbageBlock.FromRectangle(blockRect);
                page.GarbageBlocks.Add(garbageBlock);
                page.SetEditBlock(garbageBlock);
            }

            if (blockType == typeof(CommentLinkBlock))
            {
                var commentLinkBlock = CommentLinkBlock.FromRectangle(blockRect);
                page.CommentLinkBlocks.Add(commentLinkBlock);
                page.SetEditBlock(commentLinkBlock);
            }
        }
    }
}