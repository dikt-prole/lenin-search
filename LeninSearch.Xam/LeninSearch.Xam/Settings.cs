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
    }
}