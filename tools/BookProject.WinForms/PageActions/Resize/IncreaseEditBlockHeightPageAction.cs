using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Resize
{
    public class IncreaseEditBlockHeightPageAction : IPageAction
    {
        private readonly int _step;

        public IncreaseEditBlockHeightPageAction(int step = 5)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.BottomRightY += _step;
                editBlock.TopLeftY -= _step;
            }
        }
    }
}