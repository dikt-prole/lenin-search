using System;
using System.Collections.Generic;
using System.Media;

namespace LeninSearch
{
    public static class Sounds
    {
        public const string LetMe = "sounds\\let_me.wav";
        public const string So = "sounds\\so.wav";
        public const string SomeMarks = "sounds\\some_marks.wav";
        public const string YesAlmost = "sounds\\yes_almost.wav";

        private static readonly Dictionary<string, int> DurationsMs = new Dictionary<string, int>
        {
            { LetMe, 3000 },
            { So, 1000 },
            { SomeMarks, 2000 },
            { YesAlmost, 3000 }
        };

        public const int PeriodMs = 3000;

        private static DateTime _lastPlayedOn = DateTime.MinValue;
        private static string _lastSound = LetMe;

        public static void Play(string sound, bool force = false)
        {
            var now = DateTime.UtcNow;
            var goneMs = (now - _lastPlayedOn).TotalMilliseconds;

            if (!force && goneMs < DurationsMs[_lastSound] + PeriodMs) return;

            _lastSound = sound;
            _lastPlayedOn = now;
            new SoundPlayer(sound).Play();
        }
    }
}