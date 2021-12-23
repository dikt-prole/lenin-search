using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using LeninSearch.Xam.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application = Android.App.Application;

namespace LeninSearch.Xam.Droid
{
    [Activity(Theme = "@style/LsTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "[ls]";

        private TextView _progressTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);
            _progressTextView = FindViewById<TextView>(Resource.Id.txtAppVersion);
            _progressTextView.Text = "";
            Settings.OneByOne = Build.VERSION.SdkInt == BuildVersionCodes.LollipopMr1;

            Task.Run(() => Startup());
        }

        private void Startup()
        {
            if (!Directory.Exists(Settings.CorpusRoot))
            {
                Directory.CreateDirectory(Settings.CorpusRoot);
                return;
            }

            var corpusFolders = Directory.GetDirectories(Settings.CorpusRoot);
            var existingCorpusIds = corpusFolders.Select(f => f.Split('\\', '/').Last()).ToList();
            var apiService = new ApiService();
            var summaryResult = apiService.GetSummaryAsync(Settings.LsiVersion).Result;

            // 1. failed to get summary case
            if (!summaryResult.Success)
            {
                foreach (var series in Settings.InitialSeries)
                {
                    var corpusId = existingCorpusIds.FirstOrDefault(id => id.StartsWith(series));
                    // corpus.json is loaded last. So if the file exists it means corpus was loaded fine.
                    if (corpusId == null || !File.Exists(Path.Combine(Settings.CorpusRoot, corpusId, "corpus.json")))
                    {
                        SetSpalshText($"Ошибка при получении данных корпуса. Ошибка: {summaryResult.Error}. Исправьте ваше подключение к интернету и перезапустите приложение.");
                        return;
                    }
                }

                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                return;
            }

            // 2. calculate corpus ids that need to be repaired
            var summary = summaryResult.Summary;
            var repairIds = existingCorpusIds.Where(id => !File.Exists(Path.Combine(Settings.CorpusRoot, id, "corpus.json"))).ToList();
            foreach (var series in Settings.InitialSeries)
            {
                if (existingCorpusIds.Any(f => f.StartsWith(series))) continue;

                var corpusId = summary.Where(ci => ci.Series == series).OrderByDescending(ci => ci.CorpusVersion).First().Id;

                repairIds.Add(corpusId);
            }

            // 3. repair
            foreach (var corpusId in repairIds)
            {
                var corpusFolder = Path.Combine(Settings.CorpusRoot, corpusId);
                if (!Directory.Exists(corpusFolder))
                {
                    Directory.CreateDirectory(corpusFolder);
                }

                var corpusItem = summary.First(ci => ci.Id == corpusId);

                foreach (var cfi in corpusItem.Files)
                {
                    SetSpalshText($"Скачиваю файл: '{corpusId}/{cfi.Path}'");
                    var filePath = Path.Combine(corpusFolder, cfi.Path);
                    if (!File.Exists(filePath))
                    {
                        var fileBytesResult = apiService.GetFileBytesAsync(corpusItem.Id, cfi.Path).Result;

                        if (!fileBytesResult.Success)
                        {
                            SetSpalshText($"Ошибка при получении файла '{corpusItem.Id}/{cfi.Path}'. Ошибка: {fileBytesResult.Error}. Исправьте ваше подключение к интернету и перезапустите приложение.");
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