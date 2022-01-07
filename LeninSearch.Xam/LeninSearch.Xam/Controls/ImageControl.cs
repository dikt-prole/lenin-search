using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace LeninSearch.Xam.Controls
{
    public class ImageControl : StackLayout
    {
        private readonly Image _image;
        private readonly ScrollView _scroll;
        private readonly Label _label;

        private const double ZoomFactor = 1.5;

        public string Source
        {
            get => _image.Source.ToString();
            set => _image.Source = value;
        }
        public double EffectiveHeightRequest
        {
            get => HeightRequest;
            set
            {
                _scroll.HeightRequest = value;
                _image.HeightRequest = value;
            }
        }
        public double EffectiveWidthRequest
        {
            get => WidthRequest;
            set
            {
                _scroll.WidthRequest = value;
                _image.WidthRequest = value;
            }
        }

        public string Title
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        public ImageControl()
        {
            Orientation = StackOrientation.Vertical;
            Spacing = 0;

            _label = new Label
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                FontSize = Settings.UI.Font.SmallFontSize,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Italic
            };
            Children.Add(_label);
            Children.Add(Divider());

            _image = new Image();
            _scroll = new ScrollView
            {
                Orientation = ScrollOrientation.Both,
                Content = _image
            };
            Children.Add(_scroll);
            Children.Add(Divider());

            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            _image.GestureRecognizers.Add(tap);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (_image.WidthRequest > WidthRequest)
            {
                _image.WidthRequest = WidthRequest;
                _image.HeightRequest = HeightRequest;
            }
            else
            {
                _image.WidthRequest = WidthRequest * ZoomFactor;
                _image.HeightRequest = HeightRequest * ZoomFactor;
            }
        }

        private StackLayout Divider()
        {
            return new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Black,
                HeightRequest = 1,
                Margin = 0
            };
        }
    }
}