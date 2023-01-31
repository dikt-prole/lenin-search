using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Clipboard
{
    public class CopyEditBlockPageAction : BlockClipboardPageAction
    {
        public override void Execute(Page page)
        {
            ProtoBlock = page.GetEditBlock();
        }
    }
}