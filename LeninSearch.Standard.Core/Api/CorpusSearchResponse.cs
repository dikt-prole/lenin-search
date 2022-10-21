using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Api
{
    public class CorpusSearchResponse
    {
        public Dictionary<string, Dictionary<string, List<SearchResultUnitResponse>>> Units { get; set; }
    }
}