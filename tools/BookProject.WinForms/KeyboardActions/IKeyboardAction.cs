using BookProject.Core.Models;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions
{
    public interface IKeyboardAction
    {
        void Execute(object sender, BookViewModel bookVm, KeyboardArgs args);
    }
}