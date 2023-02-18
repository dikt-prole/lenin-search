using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class RemoveSelectedBlockPageAction : IPageAction
    {
        public void Execute(BookViewModel bookVm)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock != null && !(selectedBlock is Page))
            {
                bookVm.RemoveBlock(selectedBlock);
            }
        }
    }
}