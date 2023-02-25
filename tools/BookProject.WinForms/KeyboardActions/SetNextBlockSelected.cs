using BookProject.Core.Models;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions
{
    public class SetNextBlockSelected : IKeyboardAction
    {
        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            bookVm.SelectNextCurrentPageBlock(sender);
        }
    }
}