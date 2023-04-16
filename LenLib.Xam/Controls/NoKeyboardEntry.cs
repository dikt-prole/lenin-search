using System;
using Xamarin.Forms;

namespace LenLib.Xam.Controls
{
    public class NoKeyboardEntry : Entry
    {
        public NoKeyboardEntry()
        {
            Focused += HideDamnKeyboard;
            Text = Settings.Query.InitialQuery;

            InputTransparent = true;
        }

        public event Action GentlyFocused;

        protected override void OnParentSet()
        {
            var parent = (View)Parent;
            if (parent != null)
            {
                var searchEntryTapRecognizer = new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        if (IsFocused) return;

                        GentlyFocus();
                    })
                };
                parent.GestureRecognizers.Add(searchEntryTapRecognizer);
            }
        }

        public new bool Focus()
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();

            return true;
        }

        private void HideDamnKeyboard(object sender, EventArgs args)
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }

        public void GentlyFocus()
        {
            var originalText = Text ?? "";
            var newText = originalText.EndsWith(" ") ? originalText.TrimEnd(' ') : originalText + " ";
            Text = newText;

            var asteriskIndex = newText.IndexOf('*');

            var textBeforeAsterisk = newText.Substring(0, asteriskIndex);
            var spaceIndex = textBeforeAsterisk.LastIndexOf(' ');

            var selectionStart = spaceIndex + 1;
            var selectionLength = asteriskIndex - spaceIndex - 1;

            Device.InvokeOnMainThreadAsync(async () =>
            {
                CursorPosition = selectionStart;
                SelectionLength = selectionLength;
            });

            GentlyFocused?.Invoke();
        }
    }
}