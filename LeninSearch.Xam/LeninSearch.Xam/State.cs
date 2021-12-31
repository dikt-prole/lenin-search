using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Xam
{
    public class State
    {
        public static IEnumerable<CorpusItem> GetCorpusItems()
        {
            var corpusFolders = Directory.GetDirectories(Settings.CorpusRoot);
            foreach (var corpusFolder in corpusFolders)
            {
                var ciJson = File.ReadAllText(Path.Combine(corpusFolder, "corpus.json"));
                yield return JsonConvert.DeserializeObject<CorpusItem>(ciJson);
            }
        }

        public string CorpusId { get; set; }
        public string ReadingFile { get; set; }
        public string SearchQuery { get; set; }
        public int CurrentParagraphResultIndex { get; set; }
        public ushort ReadingParagraphIndex { get; set; }
        public PartialParagraphSearchResult PartialParagraphSearchResult { get; set; }

        public CorpusFileItem GetReadingCorpusFileItem()
        {
            var ci = GetCurrentCorpusItem();
            return ci.LsiFiles().First(cfi => cfi.Path == ReadingFile);
        }

        public CorpusItem GetCurrentCorpusItem()
        {
            return GetCorpusItems().First(ci => ci.Id == CorpusId);
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
            return CurrentParagraphResultIndex  < PartialParagraphSearchResult.FileResults(ReadingFile).Count - 1;
        }

        public ParagraphSearchResult GetCurrentSearchParagraphResult()
        {
            if (!IsWatchingParagraphSearchResults()) return null;
            return PartialParagraphSearchResult.FileResults(ReadingFile)[CurrentParagraphResultIndex];
        }

        public bool IsReading()
        {
            return !IsWatchingParagraphSearchResults() && !string.IsNullOrEmpty(ReadingFile);
        }
    }
}
