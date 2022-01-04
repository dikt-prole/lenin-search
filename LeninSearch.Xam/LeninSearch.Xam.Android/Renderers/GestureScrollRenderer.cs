using System;
using Android.Content;
using Android.Views;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GestureScrollView), typeof(GestureScrollViewRenderer))]
namespace LeninSearch.Xam.Droid.Renderers
{
    public class GestureScrollViewRenderer : ScrollViewRenderer
    {
        readonly CustomGestureListener _listener;
        readonly GestureDetector _detector;

        public GestureScrollViewRenderer(Context context) : base(context)
        {
            _listener = new CustomGestureListener();
            _detector = new GestureDetector(context, _listener);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            if (_detector != null)
            {
                _detector.OnTouchEvent(e);
                base.DispatchTouchEvent(e);
                return true;
            }

            return base.DispatchTouchEvent(e);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            base.OnTouchEvent(ev);

            if (_detector != null)
                return _detector.OnTouchEvent(ev);

            return false;
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                _listener.OnSwipeLeft -= HandleOnSwipeLeft;
                _listener.OnSwipeRight -= HandleOnSwipeRight;
            }

            if (e.OldElement == null)
            {
                _listener.OnSwipeLeft += HandleOnSwipeLeft;
                _listener.OnSwipeRight += HandleOnSwipeRight;
            }
        }

        void HandleOnSwipeLeft(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"HandleOnSwipeLeft: {sender}");

            ((GestureScrollView) Element).OnSwipeLeft();
        }


        void HandleOnSwipeRight(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"HandleOnSwipeRight: {sender}");

            ((GestureScrollView)Element).OnSwipeRight();
        }
    
}
}