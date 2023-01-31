using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveEditBlockTopPageAction : IPageAction
    {
        private readonly int _step;

        public MoveEditBlockTopPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(Page page)
        {
            var editBlock = page.GetEditBlock();
            if (editBlock != null)
            {
                editBlock.TopLeftY -= _step;
                editBlock.BottomRightY -= _step;
            }
        }
    }
}