using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Standard.Core.Api
{
    public class SearchResponse
    {
        public bool Success => Error == null;
        public string Error { get; set; }

        // var queryResult = Results["t01"]
        public Dictionary<string, List<SearchQueryResponse>> FileResults { get; set; }


        public SearchResult ToSearchResult()
        {
            return new SearchResult
            {
                Error = Error,
                FileResults = FileResults.ToDictionary(x => x.Key, x => x.Value.Select(sqr => sqr.ToSearchQueryResult()).ToList())
            };
        }

        public static SearchResponse FromSearchResult(SearchResult searchResult)
        {
            return new SearchResponse
            {
                Error = searchResult.Error,
                FileResults = searchResult.FileResults.ToDictionary(x => x.Key,
                    x => x.Value.Select(SearchQueryResponse.FromSearchQueryResult).ToList())
            };
        }
    }
}