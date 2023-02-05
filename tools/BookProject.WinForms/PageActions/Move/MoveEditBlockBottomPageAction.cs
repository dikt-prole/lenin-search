using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveEditBlockBottomPageAction : IPageAction
    {
        private readonly int _step;

        public MoveEditBlockBottomPageAction(int step = 10)
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
                    b.TopLeftY += _step;
                    b.BottomRightY += _step;
                });
            }
        }
    }
}