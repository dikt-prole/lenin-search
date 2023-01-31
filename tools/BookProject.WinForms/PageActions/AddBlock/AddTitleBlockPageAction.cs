using System.Drawing;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.AddBlock
{
    public class AddTitleBlockPageAction : IPageAction
    {
        private readonly int _width;
        private readonly int _height;

        public AddTitleBlockPageAction(int width = 100, int height = 100)
        {
            _width = width;
            _height = height;
        }

        public void Execute(Page page)
        {
            var blockRect = new Rectangle((page.Width - _width) / 2, (page.Height - _height) / 2, _width, _height);
            var titleBlock = TitleBlock.FromRectangle(blockRect);
            page.TitleBlocks.Add(titleBlock);
            page.SetEditBlock(titleBlock);
        }
    }
}