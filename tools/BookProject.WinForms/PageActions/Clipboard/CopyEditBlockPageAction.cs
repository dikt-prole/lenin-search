using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Clipboard
{
    public class CopyEditBlockPageAction : BlockClipboardPageAction
    {
        public override void Execute(object sender, BookViewModel bookVm)
        {
            ProtoBlock = bookVm.SelectedBlock;
        }
    }
}