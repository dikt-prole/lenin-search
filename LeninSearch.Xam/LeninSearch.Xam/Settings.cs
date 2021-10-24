using System.Collections.Generic;
using LeninSearch.Xam.Core;
using Xamarin.Forms;

namespace LeninSearch.Xam
{
    public static class Settings
    {
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
            public const string Token = "токен";
            public static readonly string InitialQuery = "дикт* проле* + науч* латин*";
            public static readonly string Txt1 = $"{Token}* + {Token}*";
            public static readonly string Txt2 = $"{Token}* {Token}* + {Token}* {Token}*";
            public static readonly string Txt3 = $"{Token}* {Token}* {Token}* + {Token}* {Token}* {Token}*";
            public static readonly string Title1 = $"* {Token}* + {Token}*";
        }
    }
}