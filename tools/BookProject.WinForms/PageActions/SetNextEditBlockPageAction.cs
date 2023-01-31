using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions
{
    public class SetNextEditBlockPageAction : IPageAction
    {
        public void Execute(Page page)
        {
            page.SetNextEditBlock();
        }
    }
}