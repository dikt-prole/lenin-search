using System;
using System.Collections.Generic;

namespace LeninSearch.Standard.Core
{
    public class SearchHeading
    {
        public string Heading { get; set; }
        public string Style { get; set; }
        public string Excerpt { get; set; }
        public int SectionIndex { get; set; }
        public int ParagraphIndex { get; set; }
        public string SummaryLine { get; set; }
        public int Level { get; set; }
        public float SummaryLeftIndent { get; set; }

        public string ToJson(bool formatted)
        {
            var br = formatted ? Environment.NewLine : "";
            var tab = formatted ? "    " : "";

            var lines = new List<string>();
            lines.Add($"{tab}'heading': '{Heading}',");
            lines.Add($"{tab}'sectionIndex': {SectionIndex},");
            lines.Add($"{tab}'paragraphIndex': {ParagraphIndex},");
            lines.Add($"{tab}'style': '{Style}',");
            lines.Add($"{tab}'excerpt': '{Excerpt}',");
            lines.Add($"{tab}'summaryLine': '{SummaryLine}',");
            lines.Add($"{tab}'summaryLeftIndent': {SummaryLeftIndent},");
            lines.Add($"{tab}'level': {Level}");

            var json = "{" + br + string.Join(br, lines) + br + "}";

            return json;
        }
    }
}