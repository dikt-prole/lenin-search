using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchQueryFactory
    {
        private readonly ushort _omitSymbolsUpTo;
        private readonly int _minTokenLength;

        public SearchQueryFactory(ushort minTokenLength, ushort omitSymbolsUpTo)
        {
            _omitSymbolsUpTo = omitSymbolsUpTo;
            _minTokenLength = minTokenLength;
        }

        public IEnumerable<SearchQuery> Construct(string queryText, string[] dictionary)
        {
            if (string.IsNullOrEmpty(queryText))
            {
                yield break;
            }

            if (queryText.Contains('*') || queryText.Contains('+'))
            {
                yield return SearchQuery.Construct(queryText, dictionary);
                yield break;
            }

            var spaceSplit = queryText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (ushort spaceSplitIndex = 0; spaceSplitIndex < spaceSplit.Length; spaceSplitIndex++)
            {
                var tokenLength = spaceSplit[spaceSplitIndex].Length;
                var allowOmit = Math.Min(tokenLength - _minTokenLength, _omitSymbolsUpTo);
                for (ushort omit = 0; omit <= allowOmit; omit++)
                {
                    var omittedToken = $"{spaceSplit[spaceSplitIndex].Substring(0, tokenLength - omit)}*";

                    // omitted
                    var omittedSpaceSplit = spaceSplit
                        .Take(spaceSplitIndex)
                        .Concat(new[] { omittedToken })
                        .Concat(spaceSplit.Skip(spaceSplitIndex + 1));
                    var omittedQueryText = string.Join(' ', omittedSpaceSplit);
                    var omittedSearchQuery = SearchQuery.Construct(omittedQueryText, dictionary);
                    omittedSearchQuery.Priority = omit;
                    yield return omittedSearchQuery;

                    // omitted with plus
                    var omittedWithPlusSpaceSplit = spaceSplit
                        .Take(spaceSplitIndex)
                        .Concat(new[] { "+", omittedToken })
                        .Concat(spaceSplit.Skip(spaceSplitIndex + 1));
                    var omittedWithPlusQueryText = string.Join(' ', omittedWithPlusSpaceSplit);
                    var omittedWithPlusSearchQuery = SearchQuery.Construct(omittedWithPlusQueryText, dictionary);
                    omittedWithPlusSearchQuery.Priority = (ushort) (10 * (spaceSplit.Length - spaceSplitIndex) + omit);
                    yield return omittedWithPlusSearchQuery;
                }
            }
        }
    }
}