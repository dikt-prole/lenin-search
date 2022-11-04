using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchQueryResult
    {
        public string Query { get; set; }
        public string[] MissingTokens { get; set; }
        public ushort Priority { get; set; }
        public List<SearchUnit> Units { get; set; }
    }
}