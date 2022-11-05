using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Xam.ListItems
{
    public class SearchUnitListItem
    {
        public string File { get; set; }
        public string CorpusId { get; set; }
        public SearchUnit SearchUnit { get; set; }
        public string Title => $"{Index}. {SearchUnit.Title}";
        public string Query { get; set; }
        public string Preview => SearchUnit.Preview;
        public ushort Index { get; set; }
        public ushort MatchSpanLength => SearchUnit.MatchSpanLength;
        public ushort QueryPriority { get; set; }
        public string[] MissingTokens { get; set; }
        public string MissingTokensText => string.Join(", ", MissingTokens);
        public bool HasMissingTokens => MissingTokens.Any();
        public SearchUnitListItem Self => this;

        public static List<SearchUnitListItem> FromSearchResult(SearchResult searchResult, string corpusId)
        {
            var searchUnitListItems = new List<SearchUnitListItem>();

            foreach (var file in searchResult.FileResults.Keys)
            {
                foreach (var searchQueryResult in searchResult.FileResults[file])
                {
                    foreach (var searchUnit in searchQueryResult.Units)
                    {
                        searchUnitListItems.Add(new SearchUnitListItem
                        {
                            CorpusId = corpusId,
                            File = file,
                            SearchUnit = searchUnit,
                            Query = searchQueryResult.Query,
                            QueryPriority = searchQueryResult.Priority,
                            MissingTokens = searchQueryResult.MissingTokens
                        });
                    }
                }
            }

            searchUnitListItems = searchUnitListItems
                .OrderBy(x => x.QueryPriority)
                .ThenBy(x => x.MatchSpanLength).ToList();
            for (ushort i = 0; i < searchUnitListItems.Count; i++)
            {
                searchUnitListItems[i].Index = (ushort)(i + 1);
            }

            return searchUnitListItems;
        }
    }
}