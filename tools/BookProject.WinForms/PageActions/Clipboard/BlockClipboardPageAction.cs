using System.Drawing;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Clipboard
{
    public abstract class BlockClipboardPageAction : IPageAction
    {
        protected static Block ProtoBlock { get; set; }
        public abstract void Execute(BookViewModel bookVm);
    }
}