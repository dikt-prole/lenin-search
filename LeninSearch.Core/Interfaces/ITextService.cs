using System.Collections.Generic;

namespace LeninSearch.Core.Interfaces
{
    public interface ITextService
    {
        public void SetLevelAndSummary(SearchHeading heading, List<SummaryLine> summaryLines);
        bool LooksLikeHeadingFragment(string text);
    }
}