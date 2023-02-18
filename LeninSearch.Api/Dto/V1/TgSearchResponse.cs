using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Api.Dto.V1
{
    public class TgSearchResponse
    {
        public int TotalCount { get; set; }
        public TgSearchUnitResponse[] Units { get; set; }

        public static TgSearchResponse FromSearchResult(SearchResult searchResult, int page, int pageSize)
        {
            var searchUnits = GetAllSearchUnits(searchResult).OrderBy(u => u.Priority).ToArray();
            return new TgSearchResponse
            {
                TotalCount = searchUnits.Length,
                Units = searchUnits.Skip(page * pageSize).Take(pageSize).ToArray()
            };
        }

        private static IEnumerable<TgSearchUnitResponse> GetAllSearchUnits(SearchResult searchResult)
        {
            foreach (var path in searchResult.FileResults.Keys)
            {
                foreach (var searchQueryResult in searchResult.FileResults[path])
                {
                    foreach (var searchUnit in searchQueryResult.Units)
                    {
                        yield return new TgSearchUnitResponse
                        {
                            ParagraphIndex = searchUnit.ParagraphIndex,
                            Path = path,
                            Preview = searchUnit.Preview,
                            Title = searchUnit.Title,
                            Priority = searchQueryResult.Priority
                        };
                    }
                }
            }
        }
    }
}