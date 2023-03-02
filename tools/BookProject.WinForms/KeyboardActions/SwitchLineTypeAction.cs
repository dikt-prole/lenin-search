using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.KeyboardActions
{
    public class SwitchLineTypeAction : IKeyboardAction
    {
        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            if (!args.Control) return;

            var selectedBlock = bookVm.SelectedBlock;
            if (selectedBlock is Line line)
            {
                var targetType = line.Type == LineType.Normal ? LineType.First : LineType.Normal;
                bookVm.ModifyBlock(this, line, l => l.Type = targetType);
            }
        }
    }
}