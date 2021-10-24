using Xamarin.Forms;

namespace LeninSearch.Xam.Controls
{
    public class SimpleLsKeyButton : Button
    {
        private string _key;
        private LsKey _lsKey;
        public string Key
        {
            get => _key;
            set
            {
                _lsKey = LsKey.Values[value];
                _key = value;
                Text = _lsKey.Key;
            }
        }

        public string Paste => _lsKey.Paste;

        public SimpleLsKeyButton()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            VerticalOptions = LayoutOptions.FillAndExpand;
            Margin = new Thickness(0);
            BackgroundColor = Settings.MainColor;
            TextColor = Color.White;
        }
    }
}