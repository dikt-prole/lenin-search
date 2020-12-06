using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LeninSearch.Core.Interfaces;

namespace LeninSearch.Core.Services
{
    public class StdTextService : ITextService
    {
        
        public static readonly Regex LettersOnlyRegex = new Regex("[^А-Яа-я]");

        public void SetLevelAndSummary(SearchHeading heading, List<SummaryLine> summaryLines)
        {
            heading.SummaryLine = null;
            heading.Level = 0;
            heading.SummaryLeftIndent = 0;

            var summaryLine = summaryLines.FirstOrDefault(l => Match(heading.Heading, l));
            if (summaryLine == null || string.IsNullOrEmpty(summaryLine.Text)) return;

            heading.SummaryLine = summaryLine.Text;
            heading.Level = summaryLine.LeftIndent > 10 ? 1 : 0;
            heading.SummaryLeftIndent = summaryLine.LeftIndent;
        }

        private bool Match(string heading, SummaryLine summaryLine)
        {
            var line = summaryLine.Text;

            if (string.IsNullOrEmpty(line)) return false;

            line = LettersOnlyRegex.Replace(line, "").ToLower();
            heading = LettersOnlyRegex.Replace(heading, "").ToLower();

            if (heading.Length < 30) return line == heading;

            if (line.Length < 30) return false;

            return heading.StartsWith(line);
        }

        public bool LooksLikeHeadingFragment(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            var lettersOnly = LettersOnlyRegex.Replace(text, "");

            if (lettersOnly.Length == 0) return false;

            var lowerCount = lettersOnly.Count(char.IsLower);

            var lowerPercentage = 100 * lowerCount / lettersOnly.Length;

            return lowerPercentage < 10 && lettersOnly.Length < 80;
        }
    }
}