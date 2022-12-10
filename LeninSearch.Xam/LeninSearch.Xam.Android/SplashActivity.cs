using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using LeninSearch.Xam.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Content.PM;
using LeninSearch.Standard.Core.Api;
using Application = Android.App.Application;

namespace LeninSearch.Xam.Droid
{
    [Activity(Theme = "@style/LsTheme.Splash", MainLauncher = true, NoHistory = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        private TextView _progressTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);
            _progressTextView = FindViewById<TextView>(Resource.Id.txtAppVersion);
            _progressTextView.Text = "";

            Task.Run(Startup);
        }

        private void Startup()
        {
            if (!Directory.Exists(Settings.CorpusRoot))
            {
                Directory.CreateDirectory(Settings.CorpusRoot);
            }

            var finishedCorpusIds = Settings.GetFinishedCorpusIds();
            var apiService = new ApiClientV1(Settings.Web.Host, Settings.Web.Port, Settings.Web.TimeoutMs);
            var summaryResult = apiService.GetSummary(Settings.LsiVersion);

            // 1. failed to get summary case
            if (!summaryResult.Success)
            {
                foreach (var series in Settings.InitialSeries)
                {
                    if (finishedCorpusIds.All(cid => !cid.StartsWith(series)))
                    {
                        SetSpalshText(Settings.Misc.SplashApiError);
                        return;
                    }
                }

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                return;
            }

            // 2. calculate corpus ids that need to be repaired
            var summary = summaryResult.Summary;
            var repairCorpusIds = new List<string>(Settings.GetUnfinishedCorpusIds());
            foreach (var series in Settings.InitialSeries)
            {
                var corpusId = summary.Where(ci => ci.Series == series).OrderByDescending(ci => ci.CorpusVersion).First().Id;
                repairCorpusIds.Add(corpusId);
            }

            // 3. repair
            foreach (var corpusId in repairCorpusIds)
            {
                var corpusFolder = Path.Combine(Settings.CorpusRoot, corpusId);
                if (!Directory.Exists(corpusFolder))
                {
                    Directory.CreateDirectory(corpusFolder);
                }

                var corpusItem = summary.First(ci => ci.Id == corpusId);

                foreach (var cfi in corpusItem.Files)
                {
                    var filePath = Path.Combine(corpusFolder, cfi.Path);
                    if (!File.Exists(filePath))
                    {
                        SetSpalshText($"Скачиваю {corpusItem.Name}: '{cfi.Path}'");
                        var fileBytesResult = apiService.GetFileBytesAsync(corpusItem.Id, cfi.Path).Result;

                        if (!fileBytesResult.Success)
                        {
                            SetSpalshText(Settings.Misc.SplashApiError);
                            return;
                        }

                        File.WriteAllBytes(filePath, fileBytesResult.Bytes);
                    }
                }
            }

            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        private void SetSpalshText(string text)
        {
            RunOnUiThread(() => _progressTextView.Text = text);
        }

        public override void OnBackPressed() { }
    }
}