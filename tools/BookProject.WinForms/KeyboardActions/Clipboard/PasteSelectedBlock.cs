using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions.Clipboard
{
    public class PasteSelectedBlock : BlockClipboardKeyboardAction
    {
        public override void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            if (ProtoBlock == null || !args.Control) return;

            if (ProtoBlock is ImageBlock protoImageBlock)
            {
                bookVm.AddBlock(sender, ImageBlock.Copy(protoImageBlock), bookVm.CurrentPage);
            }

            if (ProtoBlock is TitleBlock protoTitleBlock)
            {
                bookVm.AddBlock(sender, TitleBlock.Copy(protoTitleBlock), bookVm.CurrentPage);
            }

            if (ProtoBlock is GarbageBlock protoGarbageBlock)
            {
                bookVm.AddBlock(sender, GarbageBlock.Copy(protoGarbageBlock), bookVm.CurrentPage);
            }

            if (ProtoBlock is CommentLinkBlock protoCommentLinkBlock)
            {
                bookVm.AddBlock(sender, CommentLinkBlock.Copy(protoCommentLinkBlock), bookVm.CurrentPage);
            }
        }
    }
}