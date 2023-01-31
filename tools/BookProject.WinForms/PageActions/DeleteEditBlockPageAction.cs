using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions
{
    public class DeleteEditBlockPageAction : IPageAction
    {
        public void Execute(Page page)
        {
            var editedBlock = page.GetEditBlock();
            if (editedBlock != null)
            {
                page.RemoveBlock(editedBlock);
            }
        }
    }
}