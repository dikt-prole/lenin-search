using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions.MoveResize
{
    public class MoverightOrUpsizeSelectedBlock : MoveResizeBase
    {
        public override void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock != null && !(selectedBlock is Page))
            {
                var step = GetStep(selectedBlock);
                if (args.Shift)
                {
                    bookVm.ModifyBlock(sender, selectedBlock, b =>
                    {
                        b.TopLeftX -= step;
                        b.BottomRightX += step;
                    });
                }
                else
                {
                    bookVm.ModifyBlock(sender, selectedBlock, b =>
                    {
                        b.TopLeftX += step;
                        b.BottomRightX += step;
                    });
                }
            }
        }
    }
}