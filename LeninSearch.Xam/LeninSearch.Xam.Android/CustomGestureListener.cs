using System;
using Android.Views;

namespace LeninSearch.Xam.Droid
{
    public class CustomGestureListener : GestureDetector.SimpleOnGestureListener
    {
        static readonly int SWIPE_THRESHOLD = 100;
        static readonly int SWIPE_VELOCITY_THRESHOLD = 100;

        MotionEvent mLastOnDownEvent;

        public event EventHandler OnSwipeLeft;
        public event EventHandler OnSwipeRight;

        public override bool OnDown(MotionEvent e)
        {
            mLastOnDownEvent = e;

            return true;
        }

        public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            if (e1 == null)
                e1 = mLastOnDownEvent;

            float diffY = e2.GetY() - e1.GetY();
            float diffX = e2.GetX() - e1.GetX();

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD)
                {
                    if (diffX > 0)
                        OnSwipeRight?.Invoke(this, null);
                    else
                        OnSwipeLeft?.Invoke(this, null);
                }
            }

            return base.OnFling(e1, e2, velocityX, velocityY);
        }
    }
}