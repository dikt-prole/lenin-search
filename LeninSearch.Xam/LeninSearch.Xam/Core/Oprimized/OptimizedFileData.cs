using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LeninSearch.Xam.Core.Oprimized
{
    public class OptimizedFileData
    {
        private readonly Dictionary<uint, ushort> _inversedLocalDictionary;
        private readonly List<OptimizedParagraph> _paragraphs;
        private readonly Dictionary<ushort, OptimizedParagraph> _headers;
        private readonly Dictionary<ushort, ushort> _pages;
        private readonly Dictionary<ushort, List<ushort>> _wordParagraphMap;

        private static OptimizedFileData _currentOfd;
        private static string _currentFile;

        public static OptimizedFileData Get(string filePath)
        {
            FileUtil.WaitUnzip().Wait();

            if (_currentFile == filePath && _currentOfd != null)
            {
                return _currentOfd;
            }

            _currentOfd = ArchiveUtil.LoadOptimized($"{FileUtil.CorpusFolder}/{filePath}", CancellationToken.None);
            _currentFile = filePath;

            return _currentOfd;
        }

        public static void Clear()
        {
            _currentFile = null;
            _currentOfd = null;
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
            _paragraphs[index].IsHeader = true;
        }

        public void AddPage(ushort index, ushort page)
        {
            _pages.Add(index, page);
            _paragraphs[index].IsPageNumber = true;
            _paragraphs[index].PageNumber = page;
        }

        public IEnumerable<OptimizedParagraph> FindParagraphs(SearchOptions so)
        {
            if (so.WordIndexes?.Any() != true) yield break;

            var paragraphIndexes = GetParagraphIndexes(so.WordIndexes[0]);
            for (var i = 1; i < so.WordIndexes.Count; i++)
            {
                if (!paragraphIndexes.Any()) break;

                paragraphIndexes = paragraphIndexes.Intersect(GetParagraphIndexes(so.WordIndexes[i])).ToList();
            }

            foreach (var paragraphIndex in paragraphIndexes)
            {
                var paragraph = _paragraphs[paragraphIndex];

                if (paragraph.IsPageNumber) continue;

                var paragraphText = paragraph.GetText();
                if (paragraphText.ToLower().Contains(so.MainQuery))
                {
                    yield return paragraph;
                }
            }
        }

        public IEnumerable<SearchHeaderResult> FindHeaders(SearchOptions so)
        {
            if (so.WordIndexes?.Any() != true) yield break;

            if (_headers?.Any() != true) yield break;

            var headerIndexes = GetHeaderIndexes(so.WordIndexes[0]).ToList();
            for (var i = 1; i < so.WordIndexes.Count; i++)
            {
                if (!headerIndexes.Any()) break;

                headerIndexes = headerIndexes.Intersect(GetHeaderIndexes(so.WordIndexes[i])).ToList();
            }

            foreach (var hi in headerIndexes)
            {
                var header = _headers[hi];

                var headerText = header.GetText();
                if (headerText.ToLower().Contains(so.MainQuery))
                {
                    yield return new SearchHeaderResult
                    {
                        Index = hi,
                        Text = headerText
                    };
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
                    var currentParagraphIndexes = _wordParagraphMap[localWordIndex];
                    paragraphIndexes.AddRange(currentParagraphIndexes);
                }
            }

            return paragraphIndexes.Distinct().ToList();
        }

        private IEnumerable<ushort> GetHeaderIndexes(List<uint> globalIndexes)
        {
            var localIndexes = globalIndexes.Where(_inversedLocalDictionary.ContainsKey)
                .Select(gi => _inversedLocalDictionary[gi]).ToList();

            foreach (var headerIndex in _headers.Keys)
            {
                var header = _headers[headerIndex];
                if (localIndexes.Any(li => header.LocalWordIndexes.Contains(li)))
                {
                    yield return headerIndex;
                }
            }
        }

        public OptimizedParagraph GetParagraph(ushort index)
        {
            return _paragraphs[index];
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

        public List<OptimizedParagraph> GetParagraphs(ushort index, ushort top, ushort bottom)
        {
            var paragraphs = new List<OptimizedParagraph> { _paragraphs[index] };

            for (var i = index - 1; i > 0; i--)
            {
                if (top == 0) break;

                var p = _paragraphs[i];

                if (_pages.ContainsKey((ushort)i)) continue;

                if (p.LocalWordIndexes.Count == 0) continue;

                paragraphs.Add(p);

                top--;
            }

            paragraphs.Reverse();

            for (var i = index + 1; i < _paragraphs.Count; i++)
            {
                if (bottom == 0) break;

                var p = _paragraphs[i];

                if (_pages.ContainsKey((ushort)i)) continue;

                if (p.LocalWordIndexes.Count == 0) continue;

                paragraphs.Add(p);

                bottom--;
            }

            return paragraphs;
        }

        public OptimizedParagraph GetParagraphHeader(ushort paragraphIndex)
        {
            if (_headers?.Any() != true) return null;

            var headerIndex = _headers.Keys.Where(k => k < paragraphIndex).DefaultIfEmpty().Max();

            return headerIndex == default(ushort)
                ? null
                : _headers[headerIndex];
        }

        public OptimizedParagraph GetHeader(ushort headerIndex)
        {
            if (_headers?.Any() != true) return null;

            return _headers[headerIndex];
        }

        public string GetPage(ushort paragraphIndex)
        {
            if (_pages?.Any() != true) return null;

            var minPageIndex = _pages.Keys.Where(k => k < paragraphIndex).DefaultIfEmpty().Max();
            var maxPageIndex = _pages.Keys.Where(k => k > paragraphIndex).DefaultIfEmpty().Min();

            if (minPageIndex == default(ushort)) return $"1 - {_pages[maxPageIndex]}";

            if (maxPageIndex == default(ushort)) return $"{_pages[minPageIndex]} - ...";

            var minPage = _pages[minPageIndex];

            var maxPage = _pages[maxPageIndex];

            if (maxPage - minPage == 1) return minPage.ToString();

            return $"{minPage}-{maxPage - 1}";
        }

        public Dictionary<ushort, OptimizedParagraph> GetHeaders()
        {
            return _headers;
        }

        public IEnumerable<OptimizedParagraph> GetParagraphsByHeaderIndex(ushort headerIndex)
        {
            var nextHeaderIndex = _headers.Keys.Where(k => k > headerIndex).DefaultIfEmpty().Min();
            if (nextHeaderIndex == 0)
            {
                nextHeaderIndex = (ushort)(_paragraphs.Count - 1);
            }

            for (var i = headerIndex + 1; i < nextHeaderIndex; i++)
            {
                if (_pages.ContainsKey((ushort)i)) continue;

                var p = _paragraphs[i];

                if (p.LocalWordIndexes.Count == 0) continue;

                yield return p;
            }
        }
    }
}