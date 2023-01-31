using System.Windows.Forms;
using BookProject.Core.Models.Book;
using BookProject.WinForms.PageActions.AddBlock;
using BookProject.WinForms.PageActions.Move;
using BookProject.WinForms.PageActions.Resize;

namespace BookProject.WinForms.PageActions
{
    public class PreviewKeyDownPageActionFactory
    {
        public const int MoveStep = 5;
        public const int ResizeStep = 5;
        public const int DefaultBlockWidth = 200;
        public const int DefaultBlockHeight = 200;
        public const int DefaultCommentLinkBlockWidth = 25;
        public const int DefaultCommentLinkBlockHeight = 25;

        public IPageAction Construct(PreviewKeyDownEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.Tab:
                    return new SetNextEditBlockPageAction();
                case Keys.Delete:
                    return new DeleteEditBlockPageAction();
                case Keys.Up:
                    if (args.Shift)
                    {
                        return new ResizeEditBlockTopPageAction(ResizeStep);
                    }
                    return new MoveEditBlockTopPageAction(MoveStep);
                case Keys.Down:
                    if (args.Shift)
                    {
                        return new ResizeEditBlockBottomPageAction(ResizeStep);
                    }
                    return new MoveEditBlockBottomPageAction(MoveStep);
                case Keys.Left:
                    if (args.Shift)
                    {
                        return new ResizeEditBlockLeftPageAction(ResizeStep);
                    }
                    return new MoveEditBlockLeftPageAction(MoveStep);
                case Keys.Right:
                    if (args.Shift)
                    {
                        return new ResizeEditBlockRightPageAction(ResizeStep);
                    }
                    return new MoveEditBlockRightPageAction(MoveStep);
            }

            if (args.Control)
            {
                switch (args.KeyCode)
                {
                    case Keys.I:
                        return new AddBlockPageAction<ImageBlock>(DefaultBlockWidth, DefaultBlockHeight);
                    case Keys.T:
                        return new AddBlockPageAction<TitleBlock>(DefaultBlockWidth, DefaultBlockHeight);
                    case Keys.G:
                        return new AddBlockPageAction<GarbageBlock>(DefaultBlockWidth, DefaultBlockHeight);
                    case Keys.C:
                        return new AddBlockPageAction<CommentLinkBlock>(DefaultCommentLinkBlockWidth, DefaultCommentLinkBlockHeight);
                }
            }

            return null;
        }
    }
}