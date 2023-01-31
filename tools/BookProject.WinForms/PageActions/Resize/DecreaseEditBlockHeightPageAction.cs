using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Resize
{
    public class DecreaseEditBlockHeightPageAction : IPageAction
    {
        private readonly int _step;

        public DecreaseEditBlockHeightPageAction(int step = 5)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.BottomRightY -= _step;
                editBlock.TopLeftY += _step;
            }
        }
    }
}