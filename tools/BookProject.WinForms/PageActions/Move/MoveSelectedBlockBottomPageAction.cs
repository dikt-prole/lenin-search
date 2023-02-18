using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveSelectedBlockBottomPageAction : IPageAction
    {
        private readonly int _step;

        public MoveSelectedBlockBottomPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(object sender, BookViewModel bookVm)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (!(selectedBlock is Page) && selectedBlock != null)
            {
                bookVm.ModifyBlock(sender, selectedBlock, b =>
                {
                    b.TopLeftY += _step;
                    b.BottomRightY += _step;
                });
            }
        }
    }
}