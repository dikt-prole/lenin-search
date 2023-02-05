using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class SetNextEditBlockPageAction : IPageAction
    {
        public void Execute(BookViewModel bookVm)
        {
            bookVm.SetNextEditBlock(bookVm.CurrentPage);
        }
    }
}