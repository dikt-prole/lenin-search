using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Standard.Core.OldShit
{
    public class OptimizedFileData
    {
        private readonly Dictionary<uint, ushort> _inversedLocalDictionary;

        private readonly Dictionary<ushort, List<ushort>> _localWordParagraphMap;

        private readonly List<OptimizedParagraph> _paragraphs;

        private readonly Dictionary<ushort, List<OptimizedHeading>> _headings;

        private readonly Dictionary<ushort, ushort> _pages;

        public Dictionary<uint, ushort> InversedLocalDictionary => _inversedLocalDictionary;

        public Dictionary<ushort, ushort> Pages => _pages;

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

        public IEnumerable<OptimizedHeading> Headings
        {
            get
            {
                foreach (var hBucket in _headings.Values)
                {
                    foreach (var h in hBucket)
                    {
                        yield return h;
                    }
                }
            }
        }

        public OptimizedParagraph GetParagraph(ushort index)
        {
            return _paragraphs[index];
        }

        public OptimizedParagraph GetPrevParagraph(ushort index)
        {
            for (var i = index - 1; i > 0; i--)
            {
                var p = _paragraphs[i];

                if (p.LocalWordIndexes.Count == 0) continue;

                return p;
            }

            return null;
        }

        private IEnumerable<ushort> GetHeaderIndexes(List<uint> globalIndexes)
        {
            yield break; // хуй знает

            //var localIndexes = globalIndexes.Where(_inversedLocalDictionary.ContainsKey)
            //    .Select(gi => _inversedLocalDictionary[gi]).ToList();

            //foreach (var heading in _headings)
            //{
            //    if (localIndexes.Any(li => header.LocalWordIndexes.Contains(li)))
            //    {
            //        yield return headerIndex;
            //    }
            //}
        }

        public IEnumerable<SearchHeaderResult> FindHeaders(SearchOptions so)
        {
            yield break; // хуй знает

            //if (so.WordIndexes?.Any() != true) yield break;

            //if (_headings?.Any() != true) yield break;

            //var headerIndexes = GetHeaderIndexes(so.WordIndexes[0]).ToList();
            //for (var i = 1; i < so.WordIndexes.Count; i++)
            //{
            //    if (!headerIndexes.Any()) break;

            //    headerIndexes = headerIndexes.Intersect(GetHeaderIndexes(so.WordIndexes[i])).ToList();
            //}

            //foreach (var hi in headerIndexes)
            //{
            //    var header = _headers[hi];

            //    var headerText = header.GetText();
            //    if (headerText.ToLower().Contains(so.MainQuery))
            //    {
            //        yield return new SearchHeaderResult
            //        {
            //            Index = hi,
            //            Text = headerText
            //        };
            //    }
            //}
        }

        public OptimizedParagraph GetNextParagraph(ushort index)
        {
            for (var i = index + 1; i < _paragraphs.Count; i++)
            {
                var p = _paragraphs[i];

                if (p.LocalWordIndexes.Count == 0) continue;

                return p;
            }

            return null;
        }

        public OptimizedFileData(List<uint> localDictionary)
        {
            _paragraphs = new List<OptimizedParagraph>();
            _headings = new Dictionary<ushort, List<OptimizedHeading>>();
            _localWordParagraphMap = new Dictionary<ushort, List<ushort>>();
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
                if (!_localWordParagraphMap.ContainsKey(localWordIndex))
                {
                    _localWordParagraphMap.Add(localWordIndex, new List<ushort>());
                }

                _localWordParagraphMap[localWordIndex].Add(pIndex);
            }
        }

        public void AddHeading(ushort index, OptimizedHeading h)
        {
            if (!_headings.ContainsKey(index))
            {
                _headings.Add(index, new List<OptimizedHeading>());
            }
            _headings[index].Add(h);
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
            foreach (var globalIndex in globalIndexes)
            {
                if (_inversedLocalDictionary.ContainsKey(globalIndex))
                {
                    var localIndex = _inversedLocalDictionary[globalIndex];
                    if (_localWordParagraphMap.ContainsKey(localIndex))
                    {
                        paragraphIndexes.AddRange(_localWordParagraphMap[localIndex]);
                    }
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

        public List<OptimizedHeading> GetHeadings(ushort paragraphIndex)
        {
            if (_headings?.Any() != true) return null;

            var headingIndex = _headings.Keys.Where(k => k < paragraphIndex).DefaultIfEmpty().Max();

            if (headingIndex == default(ushort)) return null;

            var totalHeadings = _headings[headingIndex].ToArray().ToList();

            while (totalHeadings.All(h => h.Index > 0))
            {
                var minLevel = totalHeadings.Min(h => h.Level);
                var minIndex = totalHeadings.Min(h => h.Index);

                var prevHeadings = Headings.Where(h => h.Index <= minIndex && h.Level < minLevel).OrderByDescending(h => h.Index).ToList();

                if (prevHeadings.Count == 0) break;;

                totalHeadings.Add(prevHeadings[0]);
            }

            return totalHeadings;
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