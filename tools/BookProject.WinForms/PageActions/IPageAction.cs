using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions
{
    public interface IPageAction
    {
        void Execute(Page page);
    }
}