using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core.Corpus;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public static class Settings
    {
        // paths
        public const int LsiVersion = 2;

        public static readonly string[] InitialSeries = {"lenin"};
        public static string CorpusRoot => Path.Combine(Path.GetTempPath(), "corpus");
        public static string BookmarkFolder => Path.Combine(Path.GetTempPath(), "bookmarks");
        public static string StateFolder => Path.Combine(Path.GetTempPath(), "state-055EEC7F");

        public static string HistoryFile = Path.Combine(Path.GetTempPath(), "history", "history.json");

        // search options
        public const int BatchSize = 8;
        public static bool OneByOne { get; set; }
        public static int? EffectiveBatchSize => OneByOne ? (int?)BatchSize : null;

        public const int TokenIndexCountCutoff = int.MaxValue;

        public const int ResultCountCutoff = 100;

        // online search
        public static class Web
        {
            public const string Host = "leninsearch.org";
            public const int Port = 5000;
            public const int TimeoutMs = 2000;
            public const string SummaryUrl = "http://leninsearch.org:5000/corpus/summary";
            public const string CorpusFileLink = "http://leninsearch.org:5000/corpus/file?corpusId=[corpusId]&file=[file]";
        }

        public static class UI
        {
            public static class Fonts
            {
                public const double ReadingFontSize = 17;
            }

            public static class Colors
            {
                public static Color MainColor => Color.FromRgb(214, 24, 31);
                public static Color ReadSearchMatchColor => new Color(1, 0, 0, 0.20);
            }

            public static bool OldAndroidJustification { get; set; }

            public const int MaxHistoryLength = 10;

            public static readonly double ScreenDensity = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        }

        public static class Misc
        {

            public const string SplashApiError = "Ошибка при запросе данных с сервера. Исправьте ваше подключение к интернету и перезапустите приложение.";
            public const string ApiError = "Ошибка при запросе данных с сервера. Исправьте ваше подключение к интернету.";
            public const string UpdateCompleteMessage = "Обновление установлено";
        }

        public static class Query
        {
            public const string Token = "__";

            public static readonly string InitialQuery = "дикт* проле* + науч* латин*";

            public static readonly string Txt1 = $"{Token}* + {Token}*";

            public static readonly string Txt2 = $"{Token}* {Token}* + {Token}* {Token}*";

            public static readonly string Txt3 = $"{Token}* {Token}* {Token}* + {Token}* {Token}* {Token}*";

            public static readonly string Title1 = $"* {Token}* + {Token}*";
        }

        public static readonly List<Tuple<string, string>> Learning = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("Чтение и работа с текстом", "https://youtu.be/6WZ8ZEvmMk0"),
            new Tuple<string, string>("О поисковом отчете", "https://youtu.be/kdfShMJn4As"),
            new Tuple<string, string>("Работа с обновлениями", "https://youtu.be/kqvR4va6CP0"),
            new Tuple<string, string>("Поисковой запрос, поиск", "https://youtu.be/aN8ACedX1NI"),
            new Tuple<string, string>("Шаринг, закладки, история поиска", "https://youtu.be/w1LQOU1Wph8"),
            new Tuple<string, string>("Просмотр видео", "https://youtu.be/PskQR3n6neo")
        };

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
            var corpusFolders = Directory.GetDirectories(Settings.CorpusRoot);

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