using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using LenLib.Standard.Core.Api;
using Application = Android.App.Application;

namespace LenLib.Xam.Droid
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
            if (!Directory.Exists(Options.CorpusRoot))
            {
                Directory.CreateDirectory(Options.CorpusRoot);
            }

            var finishedCorpusIds = Options.GetFinishedCorpusIds();
            var apiService = new ApiClientV1(Options.Web.Host, Options.Web.Port, Options.Web.TimeoutMs);
            var summaryResult = apiService.GetSummary(Options.LsiVersion);

            // 1. failed to get summary case
            if (!summaryResult.Success)
            {
                foreach (var series in Options.InitialSeries)
                {
                    if (finishedCorpusIds.All(cid => !cid.StartsWith(series)))
                    {
                        SetSpalshText(Options.Misc.SplashApiError);
                        return;
                    }
                }

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                return;
            }

            // 2. calculate corpus ids that need to be repaired
            var summary = summaryResult.Summary;
            var repairCorpusIds = new List<string>(Options.GetUnfinishedCorpusIds());
            foreach (var series in Options.InitialSeries)
            {
                var corpusId = summary.Where(ci => ci.Series == series).OrderByDescending(ci => ci.CorpusVersion).First().Id;
                repairCorpusIds.Add(corpusId);
            }

            // 3. repair
            foreach (var corpusId in repairCorpusIds)
            {
                var corpusFolder = Path.Combine(Options.CorpusRoot, corpusId);
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
                            SetSpalshText(Options.Misc.SplashApiError);
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