using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Core
{
    public class FileData
    {
        public Dictionary<ushort, string> Headers { get; set; }
        public List<string> Paragraphs { get; set; }
        public Dictionary<string, List<ushort>> Index { get; set; }
        public Dictionary<ushort, ushort> Pages { get; set; }

        public string GetText(int paragraph, int bottomContext, int topContext)
        {
            var topParagraphs = Paragraphs.Take(paragraph).Where(p => !string.IsNullOrWhiteSpace(p));
            topParagraphs = Enumerable.Reverse(Enumerable.Reverse(topParagraphs).Take(topContext));

            var bottomParagraphs = Paragraphs.Skip(paragraph + 1).Where(p => !string.IsNullOrWhiteSpace(p));
            bottomParagraphs = bottomParagraphs.Take(bottomContext);

            var paragraphs = topParagraphs.Concat(new[] {Paragraphs[paragraph]}).Concat(bottomParagraphs).ToList();

            var text = string.Join(Environment.NewLine + Environment.NewLine, paragraphs);

            var pages = GetPages(paragraph);

            var heading = GetHeading(paragraph);

            var hr = new string(Enumerable.Range(0, 100).Select(x => '-').ToArray());

            var br = Environment.NewLine;

            return pages == null
                ? $"{heading}{br}{hr}{br}{br}{text}"
                : $"{heading} - {pages}{br}{hr}{br}{br}{text}";
        }

        private string GetPages(int paragraph)
        {
            if (Pages?.Any() != true) return null;

            var minPageKey = Pages.Keys.Where(k => k <= paragraph).OrderByDescending(k => k).FirstOrDefault();
            var maxPageKey = Pages.Keys.Where(k => k >= paragraph).OrderBy(k => k).FirstOrDefault();

            if (minPageKey == default(ushort))
            {
                return $"стр. < {Pages[maxPageKey]}";
            }

            if (maxPageKey == default(ushort))
            {
                return $"стр. > {Pages[minPageKey]}";
            }

            if (minPageKey == maxPageKey) return $"стр. {Pages[minPageKey]}";

            return $"стр. {Pages[minPageKey]}-{Pages[maxPageKey] - 1}";
        }

        private string GetHeading(int paragraph)
        {
            if (Headers?.Any() != true) return "HEADING NOT FOUND";

            var headerKey = Headers.Keys.Where(k => k < paragraph).OrderByDescending(x => x).FirstOrDefault();
            if (headerKey > 0)
            {
                return Headers[headerKey];
            }

            return "HEADING NOT FOUND";
        }
    }
}