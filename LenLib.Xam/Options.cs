using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LenLib.Standard.Core.Corpus;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LenLib.Xam
{
    public static class Options
    {
        // paths
        public const int LsiVersion = 2;

        public static readonly string[] InitialSeries = {"lenin"};
        public static string CorpusRoot => Path.Combine(Path.GetTempPath(), "corpus");
        public static string BookmarkFolder => Path.Combine(Path.GetTempPath(), "bookmarks");
        public static string StateFolder => Path.Combine(Path.GetTempPath(), "state-055EEC7F");

        public static string HistoryFile = Path.Combine(Path.GetTempPath(), "history", "history.json");

        public static class Search
        {
            public const int MaxResultsPerBook = 50;
        }

        // online search
        public static class Web
        {
            public const string Host = "leninsearch.org";
            //public const string Host = "10.0.2.2";
            public const int Port = 5000;
            public const int TimeoutMs = 15000;
        }

        public static class UI
        {
            public static class Fonts
            {
                public const double ReadingFontSize = 17;
            }

            public static class Colors
            {
                public static Color MainColor => Color.FromRgb(65, 74, 103);

                public const string MainColorHex = "#414A67";
                public static Color ReadSearchMatchColor => new Color(65, 74, 103, 0.20);
                public static string SearchUnitHighlightColorHex = "#91A5E2";
            }

            public static bool OldAndroidJustification { get; set; }

            public const int MaxHistoryLength = 10;

            public static readonly double ScreenDensity = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        }

        public static class Misc
        {

            public const string SplashApiError = "Исправьте ваше подключение к интернету и перезапустите приложение.";
            public const string ApiError = "Ошибка при запросе данных с сервера. Исправьте ваше подключение к интернету.";
            public const string UpdateCompleteMessage = "Обновление установлено";
        }

        public static bool CorpusExists(string corpusId)
        {
            if (string.IsNullOrEmpty(corpusId)) return false;

            return Directory.Exists(Path.Combine(CorpusRoot, corpusId));
        }

        public static List<string> GetFinishedCorpusIds()
        {
            var corpusIds = new List<string>();

            if (!Directory.Exists(CorpusRoot)) return corpusIds;

            var corpusFolders = Directory.GetDirectories(CorpusRoot);

            foreach (var corpusFolder in corpusFolders)
            {
                var corpusId = corpusFolder.Split('\\', '/').Last();
                if (File.Exists(Path.Combine(CorpusRoot, corpusId, "corpus.json")))
                {
                    corpusIds.Add(corpusId);
                }
            }

            return corpusIds;
        }

        public static List<string> GetUnfinishedCorpusIds()
        {
            var corpusIds = new List<string>();

            if (!Directory.Exists(CorpusRoot)) return corpusIds;

            var corpusFolders = Directory.GetDirectories(CorpusRoot);

            foreach (var corpusFolder in corpusFolders)
            {
                var corpusId = corpusFolder.Split('\\', '/').Last();
                if (!File.Exists(Path.Combine(CorpusRoot, corpusId, "corpus.json")))
                {
                    corpusIds.Add(corpusId);
                }
            }

            return corpusIds;
        }

        public static string IconFile(string corpusId)
        {
            return Path.Combine(CorpusRoot, corpusId, "icon.png");
        }

        public static string ImageFile(string corpusId, ushort imageIndex)
        {
            return Path.Combine(CorpusRoot, corpusId, $"image{imageIndex}.jpeg");
        }

        public static CorpusFileItem GetCorpusFileItem(string corpusId, string path)
        {
            var corpusItemJson = File.ReadAllText(Path.Combine(CorpusRoot, corpusId, "corpus.json"));

            var corpusItem = JsonConvert.DeserializeObject<CorpusItem>(corpusItemJson);

            return corpusItem.GetFileByPath(path);
        }

        public static IEnumerable<CorpusItem> GetCorpusItems()
        {
            var corpusFolders = Directory.GetDirectories(Options.CorpusRoot);

            foreach (var corpusFolder in corpusFolders)
            {
                var corpusJsonFile = Path.Combine(corpusFolder, "corpus.json");
                if (File.Exists(corpusJsonFile))
                {
                    var ciJson = File.ReadAllText(corpusJsonFile);
                    yield return JsonConvert.DeserializeObject<CorpusItem>(ciJson);
                }
            }
        }
    }
}