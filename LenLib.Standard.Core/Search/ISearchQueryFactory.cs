using System.Collections.Generic;

namespace LenLib.Standard.Core.Search
{
    public interface ISearchQueryFactory
    {
        IEnumerable<SearchQuery> Construct(string queryText, string[] dictionary, SearchMode mode);
    }
}