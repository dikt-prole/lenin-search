using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions.MoveResize
{
    public abstract class MoveResizeBase : IKeyboardAction
    {
        public abstract void Execute(object sender, BookViewModel bookVm, KeyboardArgs args);

        protected int GetStep(Block block)
        {
            if (block is CommentLinkBlock)
            {
                return 1;
            }

            if (block is TitleBlock)
            {
                return 3;
            }

            if (block is GarbageBlock)
            {
                return 5;
            }

            if (block is ImageBlock)
            {
                return 20;
            }

            return 5;
        }
    }
}