using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Standard.Core.Api
{
    public class SearchQueryResponse
    {
        public string Query { get; set; }
        public string[] MissingTokens { get; set; }
        public ushort Priority { get; set; }
        public List<SearchUnitResponse> Units { get; set; }

        public SearchQueryResult ToSearchQueryResult()
        {
            return new SearchQueryResult
            {
                Query = Query,
                MissingTokens = MissingTokens,
                Priority = Priority,
                Units = Units.Select(u => u.ToSearchUnit()).ToList()
            };
        }

        public static SearchQueryResponse FromSearchQueryResult(SearchQueryResult searchQueryResult)
        {
            return new SearchQueryResponse
            {
                Query = searchQueryResult.Query,
                MissingTokens = searchQueryResult.MissingTokens,
                Priority = searchQueryResult.Priority,
                Units = searchQueryResult.Units.Select(SearchUnitResponse.FromSearchResultUnit).ToList()
            };
        }
    }
}