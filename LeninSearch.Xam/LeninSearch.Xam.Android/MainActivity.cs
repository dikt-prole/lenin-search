using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Java.Util.Zip;
using Newtonsoft.Json;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using LeninSearch.Standard.Core.Reporting;
using LeninSearch.Xam.Core;
using LeninSearch.Standard.Core;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace LeninSearch.Xam.Droid
{
    [Activity(Label = "LeninSearch.Xam", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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

            State.ClearCorpusData();

            using (var reader = new StreamReader(Assets.Open("main.json")))
            {
                var response = reader.ReadToEnd();
                var corpus = JsonConvert.DeserializeObject<Corpus>(response);
                State.AddCorpus(corpus);

                var targetCorpusFile = $"{FileUtil.CorpusFolder}/main.json";
                var needUnzip = !File.Exists(targetCorpusFile) ||
                                JsonConvert.DeserializeObject<Corpus>(File.ReadAllText(targetCorpusFile)).Version !=
                                corpus.Version;
                if (needUnzip)
                {
                    // 1. unzip
                    if (Directory.Exists(FileUtil.CorpusFolder))
                    {
                        Directory.Delete(FileUtil.CorpusFolder, true);
                    }
                    UnzipAsset("main.zip", FileUtil.CorpusFolder);

                    // 2. index
                    var lsFiles = Directory.GetFiles(FileUtil.CorpusFolder, "*.ls");
                    ConcurrentOptions.OneByOne = Build.VERSION.SdkInt == BuildVersionCodes.LollipopMr1;
                    if (ConcurrentOptions.OneByOne)
                    {
                        foreach (var lsFile in lsFiles) ConvertLsToLsi(lsFile);
                    }
                    else
                    {
                        for (var i = 0; i < lsFiles.Length; i += ConcurrentOptions.LsToLsiBatchSize)
                        {
                            var tasks = lsFiles.Skip(i).Take(ConcurrentOptions.LsToLsiBatchSize).Select(lsFile => Task.Run(() => ConvertLsToLsi(lsFile))).ToArray();
                            Task.WaitAll(tasks);
                        }                        
                    }                    
                }
            }

            Settings.OldAndroidJustification = Build.VERSION.SdkInt < BuildVersionCodes.O;

            _globalEvents = new GlobalEvents();
            _app = new App(_globalEvents);
            LoadApplication(_app);
        }

        private static void ConvertLsToLsi(string lsFile)
        {
            var lsiFileName = lsFile.Replace(".ls", ".lsi");
            var lsBytes = File.ReadAllBytes(lsFile);
            var lsData = LsUtil.LoadOptimized(lsBytes, CancellationToken.None);
            var lsiBytes = LsIndexUtil.ToLsIndexBytes(lsData);
            File.WriteAllBytes(lsiFileName, lsiBytes);
            File.Delete(lsFile);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
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

        public void UnzipAsset(string assetName, string destPath)
        {
            byte[] buffer = new byte[1024];
            int byteCount;

            var destPathDir = new Java.IO.File(destPath);
            destPathDir.Mkdirs();

            using (var assetStream = Assets.Open(assetName, Android.Content.Res.Access.Streaming))
            {
                using (var zipStream = new ZipInputStream(assetStream))
                {
                    ZipEntry zipEntry;
                    while ((zipEntry = zipStream.NextEntry) != null)
                    {
                        if (zipEntry.IsDirectory)
                        {
                            var zipDir = new Java.IO.File(Path.Combine(destPath, zipEntry.Name));
                            zipDir.Mkdirs();
                            continue;
                        }

                        using (var fileStream = new FileStream(Path.Combine(destPath, zipEntry.Name), FileMode.CreateNew))
                        {
                            while ((byteCount = zipStream.Read(buffer)) != -1)
                            {
                                fileStream.Write(buffer, 0, byteCount);
                            }
                        }
                        zipEntry.Dispose();
                    }
                }
            }
        }
    }
}