using System.Collections.Generic;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core.Corpus;
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
        public ushort ReadingParagraphIndex { get; set; }

        public string SearchQuery { get; set; }
        public SearchUnit SearchUnit { get; set; }
        public SearchResult SearchResult { get; set; }

        public CorpusFileItem GetReadingCorpusFileItem()
        {
            var ci = GetCurrentCorpusItem();
            return ci.LsiFiles().First(cfi => cfi.Path == ReadingFile);
        }

        public CorpusItem GetCurrentCorpusItem()
        {
            return GetCorpusItems().First(ci => ci.Id == CorpusId);
        }

        public bool IsWatchingSearchResults()
        {
            return SearchResult?.Units?.Any() == true;
        }

        public bool IsReading()
        {
            return !IsWatchingSearchResults() && !string.IsNullOrEmpty(ReadingFile);
        }
    }
}
