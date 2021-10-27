using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace LeninSearch.Xam.Controls
{
    public class LsKeyboard : Grid
    {
        private NoKeyboardEntry _entry;
        private SimpleLsKeyButton _type1Button;
        private SimpleLsKeyButton _type2Button;
        private SimpleLsKeyButton _type3Button;
        private SimpleLsKeyButton _type4Button;
        private SimpleLsKeyButton _switchButton;
        private ImageButton _searchButton;
        private ImageButton _backspaceButton;
        public event Action SearchClick;
        public event Action NonKeyaboardUnfocus;
        private bool _unfocus;

        public LsKeyboard()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            ColumnSpacing = 0;
            RowSpacing = 0;
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition {Height = 50},
                new RowDefinition {Height = 50},
                new RowDefinition {Height = 50},
                new RowDefinition {Height = 50},
                new RowDefinition {Height = 50}
            };
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star},
                new ColumnDefinition {Width = GridLength.Star}
            };

            var allKeys = LsKey.LsKeyValues.AllKeys().ToList();

            _type1Button = new SimpleLsKeyButton { Key = allKeys[0] };
            Children.Add(_type1Button, 0, 0);
            SetRow(_type1Button, 0);
            SetColumn(_type1Button, 0);
            SetColumnSpan(_type1Button, 2);

            _type2Button = new SimpleLsKeyButton { Key = allKeys[1] };
            Children.Add(_type2Button, 0, 0);
            SetRow(_type2Button, 0);
            SetColumn(_type2Button, 2);
            SetColumnSpan(_type2Button, 2);

            _type3Button = new SimpleLsKeyButton { Key = allKeys[2] };
            Children.Add(_type3Button, 0, 0);
            SetRow(_type3Button, 0);
            SetColumn(_type3Button, 4);
            SetColumnSpan(_type3Button, 2);

            _type4Button = new SimpleLsKeyButton { Key = allKeys[3] };
            Children.Add(_type4Button, 0, 0);
            SetRow(_type4Button, 0);
            SetColumn(_type4Button, 6);
            SetColumnSpan(_type4Button, 2);

            _switchButton = new SimpleLsKeyButton { Key = allKeys[4] };
            Children.Add(_switchButton, 0, 0);
            SetRow(_switchButton, 0);
            SetColumn(_switchButton, 8);
            SetColumnSpan(_switchButton, 2);

            _backspaceButton = new ImageButton
            {
                Source = "backspace.png",
                BackgroundColor = Settings.MainColor,
                BorderColor = Color.White,
                BorderWidth = 1,
                Padding = new Thickness(6)
            };
            Children.Add(_backspaceButton);
            SetRow(_backspaceButton, 0);
            SetColumn(_backspaceButton, 10);

            var keyRows = new List<List<string>>
            {
                LsKey.LsKeyValues.AllKeys().Skip(6).Take(11).ToList(),
                LsKey.LsKeyValues.AllKeys().Skip(6 + 11).Take(11).ToList(),
                LsKey.LsKeyValues.AllKeys().Skip(6 + 22).Take(11).ToList(),
                LsKey.LsKeyValues.AllKeys().Skip(6 + 33).Take(11).ToList()
            };

            for (var rowIndex = 0; rowIndex < keyRows.Count; rowIndex++)
            {
                for (var colIndex = 0; colIndex < 11; colIndex++)
                {
                    if (rowIndex == 4 && colIndex == 10) continue;

                    var keyButton = new SimpleLsKeyButton { Key = keyRows[rowIndex][colIndex] };
                    Children.Add(keyButton, 0, 0);
                    SetRow(keyButton, rowIndex + 1);
                    SetColumn(keyButton, colIndex);
                }
            }

            _searchButton = new ImageButton
            {
                Source = "search.png",
                BackgroundColor = Settings.MainColor,
                BorderColor = Color.White,
                BorderWidth = 1,
                Padding = new Thickness(6)
            };
            Children.Add(_searchButton);
            SetRow(_searchButton, 4);
            SetColumn(_searchButton, 10);

            SelfHide();
        }
        public void BindToEntry(NoKeyboardEntry entry)
        {
            _entry = entry;
            foreach (var keyButton in Children.OfType<SimpleLsKeyButton>())
            {
                keyButton.Clicked += KeyButtonOnClicked;
            }

            _searchButton.Clicked += SearchButtonOnClicked;

            _backspaceButton.Clicked += BackspaceButtonOnClicked;

            _entry.GentlyFocused += SelfShow;
            _entry.Unfocused += EntryOnUnfocused;
        }

        private void BackspaceButtonOnClicked(object sender, EventArgs e)
        {
            _unfocus = false;

            var cursorPosition = _entry.CursorPosition + _entry.SelectionLength;
            var before = _entry.Text.Substring(0, cursorPosition);
            var after = _entry.Text.Substring(cursorPosition);

            if (cursorPosition == 0 || _entry.Text[cursorPosition - 1] == ' ')
            {
                before = before.Length > 1 && before[^2] == ' ' 
                    ? before[..^1] 
                    : before + " ";
            }
            else
            {
                before = before[..^1];
            }

            cursorPosition = before.Length;
            _entry.Text = $"{before}{after}";
            _entry.Focus();

            Device.InvokeOnMainThreadAsync(async () =>
            {
                _entry.CursorPosition = cursorPosition;
                _entry.SelectionLength = 0;
            });
        }

        private void SearchButtonOnClicked(object sender, EventArgs e)
        {
            // cleanup
            _entry.Text = _entry.Text
                .Replace($"{Settings.Query.Token}*", "")
                .Replace("  ", " ")
                .Replace(" *", "")
                .Replace("  ", " ")
                .TrimEnd('+', ' ');

            _entry.Unfocus();
            SearchClick?.Invoke();
        }

        private void EntryOnUnfocused(object sender, FocusEventArgs e)
        {
            _unfocus = true;
            Device.InvokeOnMainThreadAsync(async () =>
            {
                await Task.Delay(50);
                if (_unfocus)
                {
                    NonKeyaboardUnfocus?.Invoke();
                    SelfHide();
                }
            });
        }

        private void KeyButtonOnClicked(object sender, EventArgs e)
        {
            _unfocus = false;
            var keyButton = sender as SimpleLsKeyButton;
            var paste = keyButton.Paste;
            if (paste == Settings.Query.Txt1 || paste == Settings.Query.Txt2 || paste == Settings.Query.Txt3 || paste == Settings.Query.Title1)
            {
                _entry.Text = paste;
                var startTokenIndex = _entry.Text.IndexOf(Settings.Query.Token);
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    _entry.CursorPosition = startTokenIndex;
                    _entry.SelectionLength = Settings.Query.Token.Length;
                });
            }
            else if (paste == "прклч")
            {
                if (string.IsNullOrEmpty(_entry.Text)) return;

                if (!_entry.Text.Contains('*')) return;

                _entry.Focus();

                var offset = _entry.CursorPosition + _entry.SelectionLength + 1;
                var asteriskIndex = offset >= _entry.Text.Length ? -1 : _entry.Text.IndexOf('*', offset);
                if (asteriskIndex < 0)
                {
                    asteriskIndex = _entry.Text.IndexOf('*');
                }

                var textBeforeAsterisk = _entry.Text.Substring(0, asteriskIndex);
                var spaceIndex = textBeforeAsterisk.LastIndexOf(' ');

                var selectionStart = spaceIndex + 1;
                var selectionLength = asteriskIndex - spaceIndex - 1;

                Device.InvokeOnMainThreadAsync(async () =>
                {
                    _entry.CursorPosition = selectionStart;
                    _entry.SelectionLength = selectionLength;
                });
            }
            else
            {
                var currentText = _entry.Text;
                if (string.IsNullOrEmpty(currentText))
                {
                    _entry.Text = keyButton.Paste;
                    Device.InvokeOnMainThreadAsync(async () =>
                    {
                        _entry.CursorPosition = keyButton.Paste.Length;
                    });
                }
                else
                {
                    var before = currentText.Substring(0, _entry.CursorPosition);
                    var after = currentText.Substring(_entry.CursorPosition);
                    if (_entry.SelectionLength > 0)
                    {
                        after = after.Substring(_entry.SelectionLength);
                    }

                    _entry.Text = $"{before}{keyButton.Paste}{after}";
                    Device.InvokeOnMainThreadAsync(async () =>
                    {
                        _entry.CursorPosition = before.Length + keyButton.Paste.Length;
                        _entry.SelectionLength = 0;
                    });
                }
            }

            SelfShow();
        }

        private void SelfShow()
        {
            IsVisible = true;
        }
        private void SelfHide()
        {
            IsVisible = false;
        }
    }
}