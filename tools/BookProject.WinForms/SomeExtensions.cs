using System.Windows.Forms;
using BookProject.Core.Models;

namespace BookProject.WinForms
{
    public static class SomeExtensions
    {
        public static KeyboardArgs ToKeyboardArgs(this KeyEventArgs keyEventArgs)
        {
            return new KeyboardArgs
            {
                KeyValue = keyEventArgs.KeyValue,
                Alt = keyEventArgs.Alt,
                Control = keyEventArgs.Control,
                Shift = keyEventArgs.Shift
            };
        }
    }
}