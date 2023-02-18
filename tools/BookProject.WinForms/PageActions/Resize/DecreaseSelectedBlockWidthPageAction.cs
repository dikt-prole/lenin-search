using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Resize
{
    public class DecreaseSelectedBlockWidthPageAction : IPageAction
    {
        private readonly int _step;

        public DecreaseSelectedBlockWidthPageAction(int step = 5)
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
                    b.BottomRightX -= _step;
                    b.TopLeftX += _step;
                });
            }
        }
    }
}