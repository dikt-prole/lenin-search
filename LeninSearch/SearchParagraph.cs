using System.Collections.Generic;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.OldShit;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch
{
    public class SearchParagraph
    {
        public SearchParagraph(string text, int sectionIndex)
        {
            Text = text;
            SectionIndex = sectionIndex;
        }

        public string Text { get; set; }
        public int SectionIndex { get; set; }
        public List<SearchHeading> Headings { get; set; }
    }
}