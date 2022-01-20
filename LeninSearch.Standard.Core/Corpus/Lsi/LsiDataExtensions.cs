using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public static class LsiDataExtensions
    {
        public static LsiParagraph GetPrevParagraph(this LsiData lsiData, ushort paragraphIndex)
        {
            var beforeParagraphIndexes = lsiData.Paragraphs.Keys.Where(i => i < paragraphIndex).ToList();

            if (!beforeParagraphIndexes.Any()) return null;

            var prevParagraphIndex = beforeParagraphIndexes.Max();

            return lsiData.Paragraphs[prevParagraphIndex];
        }

        public static LsiParagraph GetNextParagraph(this LsiData lsiData, ushort paragraphIndex)
        {
            var afterParagraphIndexes = lsiData.Paragraphs.Keys.Where(i => i > paragraphIndex).ToList();

            if (!afterParagraphIndexes.Any()) return null;

            var nextParagraphIndex = afterParagraphIndexes.Min();

            return lsiData.Paragraphs[nextParagraphIndex];
        }

        public static List<LsiParagraph> GetHeadingsDownToZero(this LsiData lsiData, ushort paragraphIndex)
        {
            var resultHeadings = new List<LsiParagraph>();

            var beforeHeadings = lsiData.HeadingParagraphs.Where(h => h.Index < paragraphIndex).ToList();

            if (beforeHeadings.Count == 0) return resultHeadings;

            resultHeadings.Add(beforeHeadings.Last());

            var currentLevel = beforeHeadings.Last().HeadingLevel;

            while (currentLevel > 0)
            {
                beforeHeadings = beforeHeadings.Where(h => h.HeadingLevel < currentLevel).ToList();

                if (!beforeHeadings.Any()) break;

                resultHeadings.Add(beforeHeadings.Last());

                currentLevel = beforeHeadings.Last().HeadingLevel;
            }

            resultHeadings = resultHeadings.OrderBy(h => h.HeadingLevel).ToList();

            return resultHeadings;
        }

        public static LsiParagraph GetClosestHeadings(this LsiData lsiData, ushort paragraphIndex)
        {
            var beforeHeadings = lsiData.HeadingParagraphs.Where(h => h.Index < paragraphIndex).ToList();

            if (beforeHeadings.Count == 0) return null;

            return beforeHeadings.Last();
        }

        public static ushort? GetClosestPage(this LsiData lsiData, ushort paragraphIndex)
        {
            var beforePages = lsiData.Pages.Where(p => p.Index < paragraphIndex).OrderBy(p => p.Index).ToList();

            if (beforePages.Count == 0) return null;

            return beforePages.Last().Number;
        }
    }
}