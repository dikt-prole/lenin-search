using Android.App;
using Android.Widget;
using LenLib.Xam.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace LenLib.Xam.Droid
{
    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}