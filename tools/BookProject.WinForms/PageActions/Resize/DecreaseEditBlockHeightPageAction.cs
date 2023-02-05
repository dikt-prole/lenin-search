using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Resize
{
    public class DecreaseEditBlockHeightPageAction : IPageAction
    {
        private readonly int _step;

        public DecreaseEditBlockHeightPageAction(int step = 5)
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
                    b.BottomRightY -= _step;
                    b.TopLeftY += _step;
                });
            }
        }
    }
}