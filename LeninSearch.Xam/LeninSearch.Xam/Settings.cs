using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public static class Settings
    {
        // paths
        public const int LsiVersion = 1;

        public static readonly string[] InitialSeries = {"lenin"};
        public static string CorpusRoot => Path.Combine(Path.GetTempPath(), "corpus");
        public static string BookmarkFolder => Path.Combine(Path.GetTempPath(), "bookmarks");
        public static string StateFolder => Path.Combine(Path.GetTempPath(), "state");

        public static string HistoryFile = Path.Combine(Path.GetTempPath(), "history", "history.json");

        // search options
        public const int BatchSize = 8;
        public static bool OneByOne { get; set; }
        public static int? EffectiveBatchSize => OneByOne ? (int?)BatchSize : null;

        public const int TokenIndexCountCutoff = int.MaxValue;

        public const int ResultCountCutoff = 50;

        // online search
        public static class OnlineSearch
        {
            public const string Host = "151.248.121.154";
            public const int Port = 5000;
            public const int TimeoutMs = 8000;
        }

        public static class UI
        {
            public static class Font
            {
                public const double SmallFontSize = 12;
                public const double NormalFontSize = 17;
                public const double LargeFontSize = 12;
            }

            public static Color MainColor => Color.FromRgb(214, 24, 31);
            public static bool OldAndroidJustification { get; set; }

            public const int MaxHistoryLength = 10;

            public const int MaxParagraphCount = 200;

            public const int ScreensPulledOnTopScroll = 3;

            public const int ScreensPulledOnBottomScroll = 3;

            public const int ButtonRotationMs = 300;

            public const int TextMenuAnimationMs = 250;

            public const int BugMenuAnimationMs = 250;

            public const int AppearMs = 200;

            public const int DisappearMs = 200;

            public const int ResultScrollFadeMs = 50;

            public const int BrowserViewHeight = 600;
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
            new Tuple<string, string>("КАК РАБОТАТЬ С ПОИСКОВЫМ ЗАПРОСОМ", "https://youtu.be/gcCWzO8UwNI"),
            new Tuple<string, string>("КАК РАБОТАТЬ С ЗАКЛАДКАМИ", "https://youtu.be/p018-wq1wlI"),
            new Tuple<string, string>("КАК ИСКАТЬ ПО ЗАГОЛОВКАМ", "https://youtu.be/sSy70Vf4TLc"),
            new Tuple<string, string>("РЕЛИЗ ТЕКУЩЕЙ ВЕРСИИ", "https://youtu.be/V0mBI9Bh2T4")
        };

        public static bool CorpusExists(string corpusId)
        {
            if (string.IsNullOrEmpty(corpusId)) return false;

            return Directory.Exists(Path.Combine(CorpusRoot, corpusId));
        }

        public static List<string> GetExistingCorpusIds()
        {
            if (!Directory.Exists(CorpusRoot)) return new List<string>();

            var corpusFolders = Directory.GetDirectories(CorpusRoot);

            return corpusFolders.Select(f => f.Split('\\', '/').Last()).ToList();
        }

        public static string IconFile(string corpusId)
        {
            return Path.Combine(CorpusRoot, corpusId, "icon.png");
        }
    }
}