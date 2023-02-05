using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Resize
{
    public class DecreaseEditBlockWidthPageAction : IPageAction
    {
        private readonly int _step;

        public DecreaseEditBlockWidthPageAction(int step = 5)
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
                    b.BottomRightX -= _step;
                    b.TopLeftX += _step;
                });
            }
        }
    }
}