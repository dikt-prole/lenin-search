using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Java.Util.Zip;
using LeninSearch.Standard.Core;
using LeninSearch.Xam.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeninSearch.Xam.Droid
{
    [Activity(Theme = "@style/LsTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "[ls]";

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            SetContentView(Resource.Layout.Splash);
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

        private static void ConvertLsToLsi(string lsFile)
        {
            try
            {
                Log.Debug(TAG, $"Converting ls to lsi: {lsFile}");
                var lsiFileName = lsFile.Replace(".ls", ".lsi");
                var lsBytes = File.ReadAllBytes(lsFile);
                var lsData = LsUtil.LoadOptimized(lsBytes, CancellationToken.None);
                var lsiBytes = LsIndexUtil.ToLsIndexBytes(lsData);
                File.WriteAllBytes(lsiFileName, lsiBytes);
                File.Delete(lsFile);
                Log.Debug(TAG, $"Done converting: {lsFile}");
            }
            catch (Exception exc)
            {
                Log.Debug(TAG, $"Error: {exc.Message}");
            }            
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Startup();
        }

        private void Startup()
        {
            State.ClearCorpusData();

            using (var reader = new StreamReader(Assets.Open("main.json")))
            {
                var response = reader.ReadToEnd();
                var corpus = JsonConvert.DeserializeObject<Corpus>(response);
                State.AddCorpus(corpus);

                var targetCorpusFile = $"{FileUtil.CorpusFolder}/main.json";

                var needUnzip = true;
                if (File.Exists(targetCorpusFile))
                {
                    var existingCorpus = JsonConvert.DeserializeObject<Corpus>(File.ReadAllText(targetCorpusFile));
                    needUnzip = existingCorpus.Version != corpus.Version;
                }

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
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public override void OnBackPressed() { }
    }
}