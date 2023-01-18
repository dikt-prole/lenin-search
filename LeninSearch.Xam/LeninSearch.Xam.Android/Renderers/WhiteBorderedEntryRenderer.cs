using System;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(WhiteBorderedEntry), typeof(WhiteBorderedEntryRenderer))]
namespace LeninSearch.Xam.Droid.Renderers
{
    public class WhiteBorderedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.NewElement == null) return;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                // this API introduced in android 10
                Control.BackgroundTintList = ColorStateList.ValueOf(Color.White);
                Control.SetTextCursorDrawable(Resource.Drawable.white_cursor);
            }
        }
    }
}