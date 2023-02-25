using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.PageActions
{
    public class MoveleftOrDownsizeSelectedBlock : IKeyboardAction
    {
        private readonly int _step;

        public MoveleftOrDownsizeSelectedBlock(int step = 10)
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
                        b.TopLeftX += _step;
                        b.BottomRightX -= _step;
                    });
                }
                else
                {
                    bookVm.ModifyBlock(sender, selectedBlock, b =>
                    {
                        b.TopLeftX -= _step;
                        b.BottomRightX -= _step;
                    });
                }
            }
        }
    }
}