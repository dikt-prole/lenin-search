using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Resize
{
    public class IncreaseSelectedBlockHeightPageAction : IPageAction
    {
        private readonly int _step;

        public IncreaseSelectedBlockHeightPageAction(int step = 5)
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
                    b.BottomRightY += _step;
                    b.TopLeftY -= _step;
                });
            }
        }
    }
}