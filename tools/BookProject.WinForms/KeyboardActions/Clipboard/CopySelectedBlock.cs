using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions.Clipboard
{
    public class CopySelectedBlock : BlockClipboardKeyboardAction
    {
        public override void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            if (args.Control && !(bookVm.SelectedBlock is Page))
            {
                ProtoBlock = bookVm.SelectedBlock;
            }
        }
    }
}