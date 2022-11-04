using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchResult
    {
        public bool Success => Error == null;
        public string Error { get; set; }

        // var queryResult = Results["t01"]
        public Dictionary<string, List<SearchQueryResult>> FileResults { get; set; }
    }
}