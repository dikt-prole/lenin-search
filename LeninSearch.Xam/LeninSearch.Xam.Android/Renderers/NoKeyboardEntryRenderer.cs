using Android.Content;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoKeyboardEntry), typeof(NoKeyboardEntryRenderer))]
namespace LeninSearch.Xam.Droid.Renderers
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