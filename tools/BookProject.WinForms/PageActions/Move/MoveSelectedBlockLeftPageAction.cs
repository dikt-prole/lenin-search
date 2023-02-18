using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveSelectedBlockLeftPageAction : IPageAction
    {
        private readonly int _step;

        public MoveSelectedBlockLeftPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(BookViewModel bookVm)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock != null && !(selectedBlock is Page))
            {
                bookVm.ModifyBlock(selectedBlock, b =>
                {
                    b.TopLeftX -= _step;
                    b.BottomRightX -= _step;
                });
            }
        }
    }
}