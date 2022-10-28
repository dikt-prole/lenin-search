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
        private double _effectiveWidth, _effectiveHeight;

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
                _effectiveHeight = value;
                _scroll.HeightRequest = value;
                _image.HeightRequest = value;
            }
        }
        public double EffectiveWidthRequest
        {
            get => WidthRequest;
            set
            {
                _effectiveWidth = value;
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

            var topDivider = Divider();
            topDivider.Margin = new Thickness(0, 0, 0, 10);
            Children.Add(topDivider);

            _image = new Image();
            _scroll = new ScrollView
            {
                Orientation = ScrollOrientation.Both,
                Content = _image
            };
            Children.Add(_scroll);

            var bottomDivider = Divider();
            bottomDivider.Margin = new Thickness(0, 10, 0, 0);
            Children.Add(bottomDivider);

            var horizontalStack = new StackLayout {Orientation = StackOrientation.Horizontal};
            horizontalStack.Children.Add(new StackLayout{HorizontalOptions = LayoutOptions.FillAndExpand});
            _label = new Label
            {
                HorizontalOptions = LayoutOptions.End,
                FontSize = Settings.UI.Font.SmallFontSize,
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Italic
            };
            horizontalStack.Children.Add(_label);
            Children.Add(horizontalStack);

            var doubleTapRecognizer = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            doubleTapRecognizer.Tapped += OnTapped;
            _image.GestureRecognizers.Add(doubleTapRecognizer);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (_image.WidthRequest > _effectiveWidth)
            {
                _image.WidthRequest = _effectiveWidth;
                _image.HeightRequest = _effectiveHeight;
                _scroll.HeightRequest = _effectiveHeight;
            }
            else
            {
                _image.WidthRequest = _effectiveWidth * ZoomFactor;
                _image.HeightRequest = _effectiveHeight * ZoomFactor;
                _scroll.HeightRequest = _effectiveHeight * ZoomFactor;
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