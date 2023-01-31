using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Resize
{
    public class DecreaseEditBlockWidthPageAction : IPageAction
    {
        private readonly int _step;

        public DecreaseEditBlockWidthPageAction(int step = 5)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.BottomRightX -= _step;
                editBlock.TopLeftX += _step;
            }
        }
    }
}