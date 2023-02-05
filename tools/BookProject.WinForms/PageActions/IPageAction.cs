using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public interface IPageAction
    {
        void Execute(BookViewModel bookVm);
    }
}