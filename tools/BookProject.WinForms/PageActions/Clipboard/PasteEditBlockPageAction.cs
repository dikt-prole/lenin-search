using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions.Clipboard
{
    public class PasteEditBlockPageAction : BlockClipboardPageAction
    {
        public override void Execute(BookViewModel bookVm)
        {
            if (ProtoBlock == null) return;

            if (ProtoBlock is ImageBlock protoImageBlock)
            {
                bookVm.AddBlock(ImageBlock.Copy(protoImageBlock), bookVm.CurrentPage);
            }

            if (ProtoBlock is TitleBlock protoTitleBlock)
            {
                bookVm.AddBlock(TitleBlock.Copy(protoTitleBlock), bookVm.CurrentPage);
            }

            if (ProtoBlock is GarbageBlock protoGarbageBlock)
            {
                bookVm.AddBlock(GarbageBlock.Copy(protoGarbageBlock), bookVm.CurrentPage);
            }

            if (ProtoBlock is CommentLinkBlock protoCommentLinkBlock)
            {
                bookVm.AddBlock(CommentLinkBlock.Copy(protoCommentLinkBlock), bookVm.CurrentPage);
            }
        }
    }
}