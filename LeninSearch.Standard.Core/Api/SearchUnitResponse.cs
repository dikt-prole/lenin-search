using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Standard.Core.Api
{
    public class SearchUnitResponse
    {
        [JsonProperty(PropertyName = "pi")]
        public ushort ParagraphIndex { get; set; }

        [JsonProperty(PropertyName = "wic")]
        public List<List<uint>> WordIndexChains { get; set; }

        [JsonProperty(PropertyName = "pvw")]
        public string Preview { get; set; }


        [JsonProperty(PropertyName = "ttl")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "msl")]
        public ushort MatchSpanLength { get; set; }

        public static SearchUnitResponse FromSearchResultUnit(SearchUnit searchUnit)
        {
            return new SearchUnitResponse
            {
                ParagraphIndex = searchUnit.ParagraphIndex,
                WordIndexChains = searchUnit.WordIndexChains.Select(wic => wic.WordIndexes).ToList(),
                Preview = searchUnit.Preview,
                Title = searchUnit.Title,
                MatchSpanLength = searchUnit.MatchSpanLength
            };
        }

        public SearchUnit ToSearchUnit()
        {
            var result = new SearchUnit(ParagraphIndex)
            {
                WordIndexChains = WordIndexChains.Select(wic => new WordIndexChain {WordIndexes = wic}).ToList(),
                Preview = Preview,
                Title = Title,
                MatchSpanLength = MatchSpanLength
            };

            return result;;
        }
    }
}