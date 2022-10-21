using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Standard.Core.Api
{
    public class SearchResultUnitResponse
    {
        [JsonProperty(PropertyName = "pi")]
        public ushort ParagraphIndex { get; set; }

        [JsonProperty(PropertyName = "wic")]
        public List<List<uint>> WordIndexChains { get; set; }

        [JsonProperty(PropertyName = "pvw")]
        public string Preview { get; set; }

        [JsonProperty(PropertyName = "ttl")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "pty")]
        public ushort Priority { get; set; }

        public static SearchResultUnitResponse FromSearchResultUnit(SearchUnit searchUnit)
        {
            return new SearchResultUnitResponse
            {
                ParagraphIndex = searchUnit.ParagraphIndex,
                WordIndexChains = searchUnit.WordIndexChains.Select(wic => wic.WordIndexes).ToList(),
                Preview = searchUnit.Preview,
                Title = searchUnit.Title
            };
        }

        public SearchUnit ToSearchResultUnit()
        {
            var result = new SearchUnit(ParagraphIndex)
            {
                WordIndexChains = WordIndexChains.Select(wic => new WordIndexChain {WordIndexes = wic}).ToList(),
                Preview = Preview,
                Title = Title,
                Priority = Priority
            };

            return result;;
        }
    }
}