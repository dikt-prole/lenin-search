using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace LeninSearch.Xam.Controls
{
    public class ImageControl : StackLayout
    {
        private Image _image;
        private ImageJoystick _joystick;

        public string Source
        {
            get => _image.Source.ToString();
            set => _image.Source = value;
        }

        public ImageControl()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            Orientation = StackOrientation.Vertical;
            Spacing = 0;
            Padding = 0;
            Margin = 0;

            _image = new Image
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            Children.Add(_image);

            _joystick = new ImageJoystick
            {
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = false
            };
            Children.Add(_joystick);

            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            GestureRecognizers.Add(tap);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            _joystick.IsVisible = !_joystick.IsVisible;

            var imageWidth = _image.Width;
            var imageHeight = _image.Height;

            Debug.WriteLine($"Image: {imageWidth}x{imageHeight}");

        }
    }
}