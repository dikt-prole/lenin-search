using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class RemoveEditBlockPageAction : IPageAction
    {
        public void Execute(BookViewModel bookVm)
        {
            var editedBlock = bookVm.CurrentPage.GetEditBlock();
            if (editedBlock != null)
            {
                bookVm.RemoveBlock(editedBlock);
            }
        }
    }
}