using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Core;
using Newtonsoft.Json;

namespace LeninSearch.Xam
{
    public class State
    {
        private static List<Corpus> _corpuses = new List<Corpus>();
        public static void AddCorpus(Corpus corpus)
        {
            _corpuses.Add(corpus);
        }

        public static void ClearCorpusData()
        {
            _corpuses.Clear();
        }

        public static IEnumerable<CorpusItem> CorpusItems => _corpuses.SelectMany(c => c.Items);

        public string CorpusName { get; set; }
        public string ReadingFile { get; set; }
        public SearchRequest SearchRequest { get; set; }
        public List<ParagraphSearchResult> ParagraphResults { get; set; }
        public int CurrentParagraphResultIndex { get; set; }
        public ushort ReadingParagraphIndex { get; set; }

        public CorpusFileItem GetReadingCorpusFileItem()
        {
            var ci = GetCurrentCorpusItem();
            return ci.Files.First(cfi => cfi.Path == ReadingFile);
        }

        public CorpusItem GetCurrentCorpusItem()
        {
            return CorpusItems.First(ci => ci.Name == CorpusName);
        }

        public bool IsWatchingSearchResults()
        {
            return ParagraphResults.Count > 0;
        }

        public bool CanGoToPrevSearchResult()
        {
            if (ParagraphResults.Count == 0) return false;

            return CurrentParagraphResultIndex > 0;
        }

        public bool CanGoToNextSearchResult()
        {
            if (ParagraphResults.Count == 0) return false;

            return CurrentParagraphResultIndex  < ParagraphResults.Count - 1;
        }

        public ParagraphSearchResult GetCurrentSearchParagraphResult()
        {
            if (ParagraphResults.Count == 0) return null;

            return ParagraphResults[CurrentParagraphResultIndex];
        }

        public bool IsReading()
        {
            return ParagraphResults.Count == 0 && !string.IsNullOrEmpty(ReadingFile);
        }
    }
}
