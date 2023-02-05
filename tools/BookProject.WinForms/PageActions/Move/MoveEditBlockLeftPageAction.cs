using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveEditBlockLeftPageAction : IPageAction
    {
        private readonly int _step;

        public MoveEditBlockLeftPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(BookViewModel bookVm)
        {
            var editBlock = bookVm.CurrentPage.GetEditBlock();
            if (editBlock != null)
            {
                bookVm.ModifyBlock(editBlock, b =>
                {
                    b.TopLeftX -= _step;
                    b.BottomRightX -= _step;
                });
            }
        }
    }
}