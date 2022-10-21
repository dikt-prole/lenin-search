using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search
{
    public interface ISearchQueryFactory
    {
        IEnumerable<SearchQuery> Construct(string queryText, string[] dictionary, SearchMode mode);
    }
}