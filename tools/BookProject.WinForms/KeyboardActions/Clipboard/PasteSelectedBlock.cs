using System.Collections.Generic;
using System.Windows.Forms;
using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions.Clipboard
{
    public class PasteSelectedBlock : BlockClipboardKeyboardAction
    {
        public override void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            if (ProtoBlock == null) return;

            var pages = new List<Page>();
            if (args.Control)
            {
                pages.Add(bookVm.CurrentPage);
            }
            else if (args.Shift)
            {
                var scopeDialog = new ImageScopeDialog();
                if (scopeDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                var protoBlockPage = bookVm.GetBlockPage(ProtoBlock);
                foreach (var page in bookVm.Book.Pages)
                {
                    if (page == protoBlockPage) continue;

                    if (scopeDialog.ImageMatch(page.Index))
                    {
                        pages.Add(page);
                    }
                }
            }

            foreach (var page in pages)
            {
                if (ProtoBlock is ImageBlock protoImageBlock)
                {
                    bookVm.AddBlock(sender, ImageBlock.Copy(protoImageBlock), page);
                }

                if (ProtoBlock is TitleBlock protoTitleBlock)
                {
                    bookVm.AddBlock(sender, TitleBlock.Copy(protoTitleBlock), page);
                }

                if (ProtoBlock is GarbageBlock protoGarbageBlock)
                {
                    bookVm.AddBlock(sender, GarbageBlock.Copy(protoGarbageBlock), page);
                }

                if (ProtoBlock is CommentLinkBlock protoCommentLinkBlock)
                {
                    bookVm.AddBlock(sender, CommentLinkBlock.Copy(protoCommentLinkBlock), page);
                }
            }
        }
    }
}