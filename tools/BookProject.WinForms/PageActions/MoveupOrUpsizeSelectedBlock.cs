using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class MoveupOrUpsizeSelectedBlock : IKeyboardAction
    {
        private readonly int _step;

        public MoveupOrUpsizeSelectedBlock(int step = 5)
        {
            _step = step;
        }

        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock != null && !(selectedBlock is Page))
            {
                if (args.Shift)
                {
                    bookVm.ModifyBlock(sender, selectedBlock, b =>
                    {
                        b.BottomRightY += _step;
                        b.TopLeftY -= _step;
                    });
                }
                else
                {
                    bookVm.ModifyBlock(sender, selectedBlock, b =>
                    {
                        selectedBlock.TopLeftY -= _step;
                        selectedBlock.BottomRightY -= _step;
                    });
                }
            }
        }
    }
}