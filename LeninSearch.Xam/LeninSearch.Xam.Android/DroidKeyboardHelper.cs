using Android.App;
using Android.Content;
using Android.Views.InputMethods;
using LeninSearch.Xam.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(DroidKeyboardHelper))]
namespace LeninSearch.Xam.Droid
{
    public class DroidKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            var context = Application.Context;
            var inputMethodManager = context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && context is Activity)
            {
                var activity = context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);
                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}