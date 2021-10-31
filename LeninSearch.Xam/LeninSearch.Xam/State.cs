using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;
using Corpus = LeninSearch.Xam.Core.Corpus;

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
        public SearchQuery SearchQuery { get; set; }
        public int CurrentParagraphResultIndex { get; set; }
        public ushort ReadingParagraphIndex { get; set; }

        public PartialParagraphSearchResult PartialParagraphSearchResult { get; set; }

        public CorpusFileItem GetReadingCorpusFileItem()
        {
            var ci = GetCurrentCorpusItem();
            return ci.Files.First(cfi => cfi.Path == ReadingFile);
        }

        public CorpusItem GetCurrentCorpusItem()
        {
            return CorpusItems.First(ci => ci.Name == CorpusName);
        }

        public bool IsWatchingParagraphSearchResults()
        {
            return PartialParagraphSearchResult?.SearchResults?.Any() == true;
        }

        public bool CanGoToPrevParagraphSearchResult()
        {
            if (!IsWatchingParagraphSearchResults()) return false;
            return CurrentParagraphResultIndex > 0;
        }

        public bool CanGoToNextParagraphSearchResult()
        {
            if (!IsWatchingParagraphSearchResults()) return false;
            return CurrentParagraphResultIndex  < PartialParagraphSearchResult.SearchResults.Count - 1;
        }

        public ParagraphSearchResult GetCurrentSearchParagraphResult()
        {
            if (!IsWatchingParagraphSearchResults()) return null;
            return PartialParagraphSearchResult.SearchResults[CurrentParagraphResultIndex];
        }

        public bool IsReading()
        {
            return !IsWatchingParagraphSearchResults() && !string.IsNullOrEmpty(ReadingFile);
        }
    }
}
