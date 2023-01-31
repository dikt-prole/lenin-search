using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Resize
{
    public class ResizeEditBlockRightPageAction : IPageAction
    {
        private readonly int _step;

        public ResizeEditBlockRightPageAction(int step = 5)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.BottomRightX += _step;
            }
        }
    }
}