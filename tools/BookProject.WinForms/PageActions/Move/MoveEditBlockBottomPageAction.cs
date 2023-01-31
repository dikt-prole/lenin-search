using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveEditBlockBottomPageAction : IPageAction
    {
        private readonly int _step;

        public MoveEditBlockBottomPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.TopLeftY += _step;
                editBlock.BottomRightY += _step;
            }
        }
    }
}