using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class SetNextEditBlockPageAction : IPageAction
    {
        public void Execute(object sender, BookViewModel bookVm)
        {
            bookVm.SetNextEditBlock(sender, bookVm.CurrentPage);
        }
    }
}