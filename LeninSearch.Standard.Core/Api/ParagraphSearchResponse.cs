using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Standard.Core.Api
{
    public class ParagraphSearchResponse
    {
        public ushort ParagraphIndex { get; set; }
        public List<List<uint>> WordIndexChains { get; set; }
        public string File { get; set; }

        public static ParagraphSearchResponse From(ParagraphSearchResult searchResult)
        {
            return new ParagraphSearchResponse
            {
                ParagraphIndex = searchResult.ParagraphIndex,
                File = searchResult.File,
                WordIndexChains = searchResult.WordIndexChains.Select(wic => wic.WordIndexes).ToList()
            };
        }

        public ParagraphSearchResult ToParagraphSearchResult()
        {
            var result = new ParagraphSearchResult(ParagraphIndex)
            {
                File = File,
                WordIndexChains = WordIndexChains.Select(wic => new WordIndexChain {WordIndexes = wic}).ToList()
            };

            return result;;
        }
    }
}