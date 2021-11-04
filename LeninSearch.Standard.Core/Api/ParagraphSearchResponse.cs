using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Standard.Core.Api
{
    public class ParagraphSearchResponse
    {
        [JsonProperty(PropertyName = "pi")]
        public ushort ParagraphIndex { get; set; }

        [JsonProperty(PropertyName = "wic")]
        public List<List<uint>> WordIndexChains { get; set; }

        [JsonProperty(PropertyName = "fi")]
        public int FileIndex { get; set; }

        public static ParagraphSearchResponse From(ParagraphSearchResult searchResult, CorpusItem corpusItem)
        {
            var corpusFileItemPaths = corpusItem.Files.Select(cfi => cfi.Path).ToList();

            return new ParagraphSearchResponse
            {
                ParagraphIndex = searchResult.ParagraphIndex,
                FileIndex = corpusFileItemPaths.IndexOf(searchResult.File),
                WordIndexChains = searchResult.WordIndexChains.Select(wic => wic.WordIndexes).ToList()
            };
        }

        public ParagraphSearchResult ToParagraphSearchResult(CorpusItem corpusItem)
        {
            var result = new ParagraphSearchResult(ParagraphIndex)
            {
                File = corpusItem.Files[FileIndex].Path,
                WordIndexChains = WordIndexChains.Select(wic => new WordIndexChain {WordIndexes = wic}).ToList()
            };

            return result;;
        }
    }
}