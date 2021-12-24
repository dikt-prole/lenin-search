using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using LeninSearch.Standard.Core.Reporting;
using Application = Android.App.Application;

namespace LeninSearch.Xam.Droid
{
    [Activity(Label = "LeninSearch", 
        Icon = "@drawable/icon", 
        Theme = "@style/MainTheme", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        WindowSoftInputMode = SoftInput.StateAlwaysHidden)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private App _app;
        private GlobalEvents _globalEvents;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetStatusBarColor(Android.Graphics.Color.Argb(180, 214, 24, 31));
            
            AppCenter.Start("82046ff3-062c-4160-870a-62dbb982859b", typeof(Analytics), typeof(Crashes));

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            //OfflineSearcher.OneByOne = Build.VERSION.SdkInt ==  BuildVersionCodes.LollipopMr1;

            ReportItem.GlobalDeviceId = Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            Settings.UI.OldAndroidJustification = Build.VERSION.SdkInt < BuildVersionCodes.O;

            _globalEvents = new GlobalEvents();
            _app = new App(_globalEvents);

            LoadApplication(_app);

            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden);
        }        

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            if (_app.AllowBackButton)
            {
                base.OnBackPressed();
            }
            else
            {
                _globalEvents.OnBackButtonPressed();
            }
        }
    }
}