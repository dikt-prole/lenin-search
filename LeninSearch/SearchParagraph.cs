using System.Collections.Generic;
using LeninSearch.Core;

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