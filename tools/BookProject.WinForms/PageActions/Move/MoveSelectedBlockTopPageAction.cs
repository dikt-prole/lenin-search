using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Move
{
    public class MoveSelectedBlockTopPageAction : IPageAction
    {
        private readonly int _step;

        public MoveSelectedBlockTopPageAction(int step = 10)
        {
            _step = step;
        }

        public void Execute(object sender, BookViewModel bookVm)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock != null && !(selectedBlock is Page))
            {
                bookVm.ModifyBlock(sender, selectedBlock, b =>
                {
                    selectedBlock.TopLeftY -= _step;
                    selectedBlock.BottomRightY -= _step;
                });
            }
        }
    }
}