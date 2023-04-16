using Android.Content;
using LenLib.Xam.Controls;
using LenLib.Xam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoKeyboardEntry), typeof(NoKeyboardEntryRenderer))]
namespace LenLib.Xam.Droid.Renderers
{
    public class NoKeyboardEntryRenderer : EntryRenderer
    {
        public NoKeyboardEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement == null) return;
            
        }
    }
}