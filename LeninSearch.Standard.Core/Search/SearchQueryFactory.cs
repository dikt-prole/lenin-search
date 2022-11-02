using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeninSearch.Standard.Core.Search.TokenVarying;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchQueryFactory : ISearchQueryFactory
    {
        private readonly RuPorter _stemmer;

        public SearchQueryFactory()
        {
            _stemmer = new RuPorter();
        }

        public IEnumerable<SearchQuery> Construct(string queryText, string[] dictionary, SearchMode mode)
        {
            if (string.IsNullOrEmpty(queryText))
            {
                yield break;
            }

            if (queryText.Contains('*') || queryText.Contains('+'))
            {
                yield return SearchQuery.Construct(queryText, dictionary, mode, 0);
                yield break;
            }

            // level 1
            var allTokens = queryText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => $"{_stemmer.Stemm(s)}*" ).ToArray();
            foreach (var searchQuery in GetQueryVariants(allTokens, 1, dictionary, mode))
            {
                yield return searchQuery;
            }

            var subsets = SubSetsOf(allTokens).Select(ss => ss.ToArray()).ToArray();
            Debug.WriteLine("Subsets:");
            foreach (var subset in subsets.OrderByDescending(s => s.Length))
            {
                Debug.WriteLine(string.Join(" ", subset));
            }

            // todo:
            // 1. use prioritized subsets
            // 2. more optimal search query construction (same word indexes for a token)

            if (allTokens.Length > 1)
            {
                for (var i = 0; i < allTokens.Length; i++)
                {
                    // level 2
                    var levelTwoSplit = allTokens.Take(i).Concat(allTokens.Skip(i + 1)).ToArray();
                    foreach (var searchQuery in GetQueryVariants(levelTwoSplit, 100, dictionary, mode))
                    {
                        yield return searchQuery;
                    }
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/999182/6483508
        /// </summary>
        public static IEnumerable<IEnumerable<T>> SubSetsOf<T>(IEnumerable<T> source)
        {
            if (!source.Any())
                return Enumerable.Repeat(Enumerable.Empty<T>(), 1);

            var element = source.Take(1);

            var haveNots = SubSetsOf(source.Skip(1));
            var haves = haveNots.Select(set => element.Concat(set));

            return haves.Concat(haveNots);
        }

        private IEnumerable<SearchQuery> GetQueryVariants(string[] tokens, ushort basePriority, string[] dictionary, SearchMode mode)
        {
            yield return SearchQuery.Construct(string.Join(' ', tokens), dictionary, mode, basePriority);
            for (var tokenIndex = 1; tokenIndex < tokens.Length; tokenIndex++)
            {
                var variedWithPlusTokens = tokens.Take(tokenIndex)
                    .Concat(new[] { "+" })
                    .Concat(tokens.Skip(tokenIndex));

                var priority = (ushort)(basePriority + 10 * (tokens.Length - tokenIndex));
                var variedWithPlusQuery = SearchQuery.Construct(string.Join(' ', variedWithPlusTokens), dictionary, mode, priority);

                yield return variedWithPlusQuery;
            }
        }
    }
}