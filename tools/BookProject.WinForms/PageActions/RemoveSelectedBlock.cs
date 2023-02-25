using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class RemoveSelectedBlock : IKeyboardAction
    {
        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock != null && !(selectedBlock is Page))
            {
                bookVm.SetNextBlockSelected(sender);
                bookVm.RemoveBlock(sender, selectedBlock);
            }
        }
    }
}