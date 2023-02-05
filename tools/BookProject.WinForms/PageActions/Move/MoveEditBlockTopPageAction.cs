using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveEditBlockTopPageAction : IPageAction
    {
        private readonly int _step;

        public MoveEditBlockTopPageAction(int step = 10)
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
                    editBlock.TopLeftY -= _step;
                    editBlock.BottomRightY -= _step;
                });
            }
        }
    }
}