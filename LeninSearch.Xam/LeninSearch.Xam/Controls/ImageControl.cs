using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace LeninSearch.Xam.Controls
{
    public class ImageControl : ScrollView
    {
        private Image _image;

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
                HeightRequest = value;
                _image.HeightRequest = value;
            }
        }
        public double EffectiveWidthRequest
        {
            get => WidthRequest;
            set
            {
                WidthRequest = value;
                _image.WidthRequest = value;
            }
        }

        public ImageControl()
        {
            _image = new Image();
            Orientation = ScrollOrientation.Both;
            Content = _image;

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
    }
}