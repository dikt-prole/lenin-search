using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search.TokenVarying;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchQueryFactory : ISearchQueryFactory
    {
        private readonly ITokenVariantProvider _tokenVariantProvider;

        public SearchQueryFactory(ITokenVariantProvider tokenVariantProvider)
        {
            _tokenVariantProvider = tokenVariantProvider;
        }

        public IEnumerable<SearchQuery> Construct(string queryText, string[] dictionary, SearchMode mode)
        {
            if (string.IsNullOrEmpty(queryText))
            {
                yield break;
            }

            if (queryText.Contains('*') || queryText.Contains('+'))
            {
                yield return SearchQuery.Construct(queryText, dictionary, mode);
                yield break;
            }

            var initialSplit = new SearchSplit
            {
                Priority = 0,
                Tokens = queryText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            };

            var variedSplits = VarySplit(initialSplit);

            foreach (var variedSplit in variedSplits)
            {
                var variedQuery = SearchQuery.Construct(string.Join(' ', variedSplit.Tokens), dictionary, mode);
                variedQuery.Priority = variedSplit.Priority;

                yield return variedQuery;

                for (var tokenIndex = 0; tokenIndex < variedSplit.Tokens.Length; tokenIndex++)
                {
                    var variedWithPlusTokens = variedSplit.Tokens.Take(tokenIndex)
                        .Concat(new[] { "+" })
                        .Concat(variedSplit.Tokens.Skip(tokenIndex));

                    var variedWithPlusQuery = SearchQuery.Construct(string.Join(' ', variedWithPlusTokens), dictionary, mode);

                    variedWithPlusQuery.Priority = (ushort)(variedSplit.Priority + 10 * (variedSplit.Tokens.Length - tokenIndex));

                    yield return variedWithPlusQuery;
                }
            }
        }

        private IEnumerable<SearchSplit> VarySplit(SearchSplit split)
        {
            var zeroIndexTokens = _tokenVariantProvider.Vary(split.Tokens[0]).ToList();
            if (split.Tokens.Length == 1)
            {
                foreach (var zeroIndexToken in zeroIndexTokens)
                {
                    yield return new SearchSplit
                    {
                        Priority = zeroIndexToken.Omit,
                        Tokens = new[] { zeroIndexToken.Token }
                    };
                }

                yield break;
            }

            var smallerSplit = new SearchSplit
            {
                Tokens = split.Tokens.Skip(1).ToArray(),
                Priority = 0
            };

            var variedSplits = VarySplit(smallerSplit).ToList();
            foreach (var zeroIndexToken in zeroIndexTokens)
            {
                foreach (var variedSplit in variedSplits)
                {
                    yield return new SearchSplit
                    {
                        Tokens = new[] { zeroIndexToken.Token }.Concat(variedSplit.Tokens).ToArray(),
                        Priority = (ushort)(zeroIndexToken.Omit + variedSplit.Priority)
                    };
                }
            }
        }
    }
}