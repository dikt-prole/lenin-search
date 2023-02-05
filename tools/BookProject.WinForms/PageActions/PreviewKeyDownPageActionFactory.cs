using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.WinForms.PageActions.AddBlock;
using BookProject.WinForms.PageActions.Clipboard;
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
                    return new RemoveEditBlockPageAction();
                case Keys.Up:
                    if (args.Shift)
                    {
                        return new IncreaseEditBlockHeightPageAction(ResizeStep);                        
                    }
                    return new MoveEditBlockTopPageAction(MoveStep);
                case Keys.Down:
                    if (args.Shift)
                    {
                        return new DecreaseEditBlockHeightPageAction(ResizeStep);
                    }
                    return new MoveEditBlockBottomPageAction(MoveStep);
                case Keys.Left:
                    if (args.Shift)
                    {
                        return new DecreaseEditBlockWidthPageAction(ResizeStep);
                    }
                    return new MoveEditBlockLeftPageAction(MoveStep);
                case Keys.Right:
                    if (args.Shift)
                    {
                        return new IncreaseEditBlockWidthPageAction(ResizeStep);
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
                    case Keys.N:
                        return new AddBlockPageAction<CommentLinkBlock>(DefaultCommentLinkBlockWidth, DefaultCommentLinkBlockHeight);
                    case Keys.C:
                        return new CopyEditBlockPageAction();
                    case Keys.V:
                        return new PasteEditBlockPageAction();
                }
            }

            return null;
        }
    }
}