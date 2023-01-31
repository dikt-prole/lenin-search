using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveEditBlockLeftPageAction : IPageAction
    {
        private readonly int _step;

        public MoveEditBlockLeftPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.TopLeftX -= _step;
                editBlock.BottomRightX -= _step;
            }
        }
    }
}