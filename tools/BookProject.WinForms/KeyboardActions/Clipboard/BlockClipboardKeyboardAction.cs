using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions.Clipboard
{
    public abstract class BlockClipboardKeyboardAction : IKeyboardAction
    {
        protected static Block ProtoBlock { get; set; }
        public abstract void Execute(object sender, BookViewModel bookVm, KeyboardArgs args);
    }
}