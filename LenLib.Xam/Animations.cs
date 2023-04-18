using Xamarin.Forms;

namespace LenLib.Xam
{
    public static class Animations
    {
        public static void OpacityToZeroAndBack(View view, uint periodMs = 800)
        {
            var animation = new Animation
            {
                { 0, 0.4, new Animation(v => view.Opacity = v, 1, 0) },
                { 0.4, 0.8, new Animation(v => view.Opacity = v, 0, 1) }
            };
            animation.Commit(view, "OpacityToZeroAndBack", 16, periodMs);
        }
    }
}