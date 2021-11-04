using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public static class Settings
    {
        // paths
        public const int CorpusVersion = 1;
        public static string CorpusFolder => Path.Combine(Path.GetTempPath(), $"LeninSearch_Corpus_{CorpusVersion}");
        public static string BookmarkFolder => Path.Combine(Path.GetTempPath(), $"LeninSearch_Bookmarks");
        public static string StateFolder => Path.Combine(Path.GetTempPath(), $"LeninSearch_State");

        // concurrent
        public const int BatchSize = 8;
        public static bool OneByOne { get; set; }
        public static int? EffectiveBatchSize => OneByOne ? (int?)BatchSize : null;

        // online search
        public static class OnlineSearch
        {
            public const string Host = "151.248.121.154";
            public const int Port = 5000;
            public const int TimeoutMs = 2000;
        }

        // ui
        public const double MainFontSize = 17;

        public const double SummaryFontSize = 17;
        public static Color MainColor => Color.FromRgb(214, 24, 31);
        public static bool OldAndroidJustification { get; set; }

        public const int MaxParagraphCount = 200;

        public const int ScreensPulledOnTopScroll = 3;

        public const int ScreensPulledOnBottomScroll = 3;

        public const int ButtonRotationMs = 300;

        public const int TextMenuAnimationMs = 250;

        public const int BugMenuAnimationMs = 250;

        public const int AppearMs = 200;

        public const int DisappearMs = 200;

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
    }
}