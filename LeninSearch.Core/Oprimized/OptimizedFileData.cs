using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace LeninSearch.Core.Oprimized
{
    public class OptimizedFileData
    {
        private readonly Dictionary<uint, ushort> _inversedLocalDictionary;
        private readonly List<OptimizedParagraph> _paragraphs;
        private readonly Dictionary<ushort, OptimizedParagraph> _headers;
        private readonly Dictionary<ushort, ushort> _pages;
        private readonly Dictionary<ushort, List<ushort>> _wordParagraphMap;

        public IEnumerable<OptimizedParagraph> Paragraphs
        {
            get
            {
                foreach (var op in _paragraphs)
                {
                    yield return op;
                }
            }
        }

        public IEnumerable<OptimizedParagraph> Headers
        {
            get
            {
                foreach (var h in _headers)
                {
                    yield return h.Value;
                }
            }
        }

        public IEnumerable<ushort> Pages
        {
            get
            {
                foreach (var p in _pages)
                {
                    yield return p.Value;
                }
            }
        }

        public OptimizedFileData(List<uint> localDictionary)
        {
            _paragraphs = new List<OptimizedParagraph>();
            _headers = new Dictionary<ushort, OptimizedParagraph>();
            _wordParagraphMap = new Dictionary<ushort, List<ushort>>();
            _inversedLocalDictionary = new Dictionary<uint, ushort>();
            _pages = new Dictionary<ushort, ushort>();
            for (ushort i = 0; i < localDictionary.Count; i++)
            {
                _inversedLocalDictionary.Add(localDictionary[i], i);
            }

            return;
        }

        public void AddParagraph(OptimizedParagraph p)
        {
            _paragraphs.Add(p);
            var pIndex = (ushort) (_paragraphs.Count - 1);
            p.Index = pIndex;
            foreach (var localWordIndex in p.LocalWordIndexes)
            {
                if (!_wordParagraphMap.ContainsKey(localWordIndex))
                {
                    _wordParagraphMap.Add(localWordIndex, new List<ushort>());
                }

                _wordParagraphMap[localWordIndex].Add(pIndex);
            }
        }

        public void AddHeader(ushort index, OptimizedParagraph h)
        {
            _headers.Add(index, h);
        }

        public void AddPage(ushort index, ushort page)
        {
            _pages.Add(index, page);
        }

        public IEnumerable<OptimizedParagraph> FindParagraphs(SearchOptions so, string[] dictionary)
        {
            if (so.WordIndexes?.Any() != true) yield break;

            var paragraphIndexes = GetParagraphIndexes(so.WordIndexes[0]);
            for (var i = 1; i < so.WordIndexes.Count; i++)
            {
                paragraphIndexes = paragraphIndexes.Intersect(GetParagraphIndexes(so.WordIndexes[i])).ToList();
            }

            foreach (var paragraphIndex in paragraphIndexes)
            {
                var paragraph = _paragraphs[paragraphIndex];
                var paragraphText = paragraph.GetText(dictionary);
                if (paragraphText.ToLower().Contains(so.MainQuery))
                {
                    yield return paragraph;
                }
            }
        }

        private List<ushort> GetParagraphIndexes(List<uint> globalIndexes)
        {
            var paragraphIndexes = new List<ushort>();
            foreach (var gi in globalIndexes)
            {
                if (_inversedLocalDictionary.ContainsKey(gi))
                {
                    var localWordIndex = _inversedLocalDictionary[gi];
                    paragraphIndexes.AddRange(_wordParagraphMap[localWordIndex]);
                }
            }

            return paragraphIndexes.Distinct().ToList();
        }

        public List<OptimizedParagraph> GetParagraphs(ushort index, ushort top, ushort bottom, string[] dictionary)
        {
            var paragraphs = new List<OptimizedParagraph> { _paragraphs[index] };

            for (var i = index - 1; i > 0; i--)
            {
                if (top == 0) break;

                var p = _paragraphs[i];

                if (string.IsNullOrWhiteSpace(p.GetText(dictionary))) continue;

                paragraphs.Add(p);

                top--;
            }

            paragraphs.Reverse();

            for (var i = index + 1; i < _paragraphs.Count; i++)
            {
                if (bottom == 0) break;

                var p = _paragraphs[i];

                if (string.IsNullOrWhiteSpace(p.GetText(dictionary))) continue;

                paragraphs.Add(p);

                bottom--;
            }

            return paragraphs;
        }

        public OptimizedParagraph GetHeader(ushort paragraphIndex)
        {
            if (_headers?.Any() != true) return null;

            var headerIndex = _headers.Keys.Where(k => k < paragraphIndex).DefaultIfEmpty().Max();

            return headerIndex == default(ushort)
                ? null
                : _headers[headerIndex];
        }

        public string GetPage(ushort paragraphIndex)
        {
            if (_pages?.Any() != true) return null;

            var minPageIndex = _pages.Keys.Where(k => k < paragraphIndex).DefaultIfEmpty().Max();
            var maxPageIndex = _pages.Keys.Where(k => k > paragraphIndex).DefaultIfEmpty().Min();

            if (minPageIndex == default(ushort)) return $"<{_pages[maxPageIndex]}";

            if (maxPageIndex == default(ushort)) return $">{_pages[minPageIndex]}";

            var minPage = _pages[minPageIndex];

            var maxPage = _pages[maxPageIndex];

            if (maxPage - minPage == 1) return minPage.ToString();

            return $"{minPage}-{maxPage - 1}";
        }
    }
}