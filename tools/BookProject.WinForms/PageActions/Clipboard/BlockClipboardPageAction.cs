using System.Drawing;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Clipboard
{
    public abstract class BlockClipboardPageAction : IPageAction
    {
        protected static Block ProtoBlock { get; set; }
        public abstract void Execute(Page page);
    }
}