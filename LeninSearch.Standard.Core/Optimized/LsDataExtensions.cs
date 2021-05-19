using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Optimized
{
    public static class LsDataExtensions
    {
        public static LsParagraph GetPrevParagraph(this LsData lsData, ushort paragraphIndex)
        {
            var beforeParagraphIndexes = lsData.Paragraphs.Keys.Where(i => i < paragraphIndex).ToList();

            if (!beforeParagraphIndexes.Any()) return null;

            var prevParagraphIndex = beforeParagraphIndexes.Max();

            return lsData.Paragraphs[prevParagraphIndex];
        }

        public static LsParagraph GetNextParagraph(this LsData lsData, ushort paragraphIndex)
        {
            var afterParagraphIndexes = lsData.Paragraphs.Keys.Where(i => i > paragraphIndex).ToList();

            if (!afterParagraphIndexes.Any()) return null;

            var nextParagraphIndex = afterParagraphIndexes.Min();

            return lsData.Paragraphs[nextParagraphIndex];
        }

        public static List<LsHeading> GetHeadingsDownToZero(this LsData lsData, ushort paragraphIndex)
        {
            var resultHeadings = new List<LsHeading>();

            var beforeHeadings = lsData.Headings.Where(h => h.Index < paragraphIndex).OrderBy(h => h.Index).ToList();

            if (beforeHeadings.Count == 0) return resultHeadings;

            resultHeadings.Add(beforeHeadings.Last());

            var currentLevel = beforeHeadings.Last().Level;

            while (currentLevel > 0)
            {
                beforeHeadings = beforeHeadings.Where(h => h.Level < currentLevel).ToList();

                resultHeadings.Add(beforeHeadings.Last());

                currentLevel = beforeHeadings.Last().Level;
            }

            resultHeadings = resultHeadings.OrderBy(h => h.Level).ToList();

            return resultHeadings;
        }

        public static LsHeading GetClosestHeadings(this LsData lsData, ushort paragraphIndex)
        {
            var beforeHeadings = lsData.Headings.Where(h => h.Index < paragraphIndex).OrderBy(h => h.Index).ToList();

            if (beforeHeadings.Count == 0) return null;

            return beforeHeadings.Last();
        }

        public static ushort? GetClosestPage(this LsData lsData, ushort paragraphIndex)
        {
            var beforePages = lsData.Pages.Where(kvp => kvp.Key < paragraphIndex).OrderBy(kvp => kvp.Key).ToList();

            if (beforePages.Count == 0) return null;

            return beforePages.Last().Value;
        }
    }
}