using System.Diagnostics;

namespace LenLib.Standard.Core
{
    public class FancyStopwatch : Stopwatch
    {
        private string _section;

        public void Start(string section)
        {
            Reset();
            _section = section;
            Start();
        }

        public void StopAndReport()
        {
            Stop();
            Debug.WriteLine($"{_section} took {ElapsedMilliseconds}ms");
        }
    }
}