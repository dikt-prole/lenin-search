using BookProject.Core.Models.Book;

namespace BookProject.WinForms.PageActions.Clipboard
{
    public class PasteEditBlockPageAction : BlockClipboardPageAction
    {
        public override void Execute(Page page)
        {
            if (ProtoBlock == null) return;

            if (ProtoBlock is ImageBlock protoImageBlock)
            {
                var imageBlock = ImageBlock.Copy(protoImageBlock);
                page.ImageBlocks.Add(imageBlock);
                page.SetEditBlock(imageBlock);
            }

            if (ProtoBlock is TitleBlock protoTitleBlock)
            {
                var titleBlock = TitleBlock.Copy(protoTitleBlock);
                page.TitleBlocks.Add(titleBlock);
                page.SetEditBlock(titleBlock);
            }

            if (ProtoBlock is GarbageBlock protoGarbageBlock)
            {
                var garbageBlock = GarbageBlock.Copy(protoGarbageBlock);
                page.GarbageBlocks.Add(garbageBlock);
                page.SetEditBlock(garbageBlock);
            }

            if (ProtoBlock is CommentLinkBlock protoCommentLinkBlock)
            {
                var commentLinkBlock = CommentLinkBlock.Copy(protoCommentLinkBlock);
                page.CommentLinkBlocks.Add(commentLinkBlock);
                page.SetEditBlock(commentLinkBlock);
            }
        }
    }
}