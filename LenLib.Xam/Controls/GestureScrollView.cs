using System;
using Xamarin.Forms;

namespace LenLib.Xam.Controls
{
    public class GestureScrollView : ScrollView
    {
        public event EventHandler SwipeLeft;
        public event EventHandler SwipeRight;

        public void OnSwipeLeft() =>
            SwipeLeft?.Invoke(this, null);

        public void OnSwipeRight() =>
            SwipeRight?.Invoke(this, null);
    }
}