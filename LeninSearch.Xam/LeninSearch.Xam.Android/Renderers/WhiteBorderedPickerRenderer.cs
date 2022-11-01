using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(WhiteBorderedPicker), typeof(WhiteBorderedPickerRenderer))]
namespace LeninSearch.Xam.Droid.Renderers
{
    public class WhiteBorderedPickerRenderer : PickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Control.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.White);
                Control.SetTextCursorDrawable(Resource.Drawable.white_cursor);
            }
            else
            {
                Control.Background.SetColorFilter(Android.Graphics.Color.White, PorterDuff.Mode.SrcAtop);
                Control.SetTextCursorDrawable(Resource.Drawable.white_cursor);
                Control.SetHintTextColor(Color.White);
            }
        }
    }
}