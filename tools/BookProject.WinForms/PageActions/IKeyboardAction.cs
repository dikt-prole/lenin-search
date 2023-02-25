using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public interface IKeyboardAction
    {
        void Execute(object sender, BookViewModel bookVm, KeyboardArgs args);
    }
}