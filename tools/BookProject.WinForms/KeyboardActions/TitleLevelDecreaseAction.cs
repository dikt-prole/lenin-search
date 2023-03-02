using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions
{
    public class TitleLevelDecreaseAction : IKeyboardAction
    {
        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock is TitleBlock titleBlock)
            {
                bookVm.ModifyBlock(this, titleBlock, tb => tb.Level -= 1);
            }
        }
    }
}