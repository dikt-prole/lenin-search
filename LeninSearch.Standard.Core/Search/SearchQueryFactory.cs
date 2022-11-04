using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Search.TokenVarying;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchQueryFactory : ISearchQueryFactory
    {
        private readonly IStemmer _stemmer;
        private const int OmitTokensMaxCount = 2;
        public SearchQueryFactory(IStemmer stemmer)
        {
            _stemmer = stemmer;
        }

        public IEnumerable<SearchQuery> Construct(string queryText, string[] dictionary, SearchMode mode)
        {
            if (string.IsNullOrEmpty(queryText))
            {
                yield break;
            }

            // exact search case
            if (queryText.Contains('*') || queryText.Contains('+'))
            {
                if (queryText.Contains('+'))
                {
                    var plusSplit = queryText.Split('+', StringSplitOptions.RemoveEmptyEntries);
                    var orderedStems = plusSplit[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim('*')).ToArray();
                    var nonOrderedStems = plusSplit[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim('*')).ToArray();
                    var exactSearchStemWordIndexes =
                        GetStemWordIndexes(dictionary, orderedStems.Concat(nonOrderedStems).ToArray());
                    yield return ConstructSearchQuery(orderedStems, nonOrderedStems, 1, mode,
                        exactSearchStemWordIndexes);
                }
                else
                {
                    var orderedStems = queryText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim('*')).ToArray();
                    var exactSearchStemWordIndexes =
                        GetStemWordIndexes(dictionary, orderedStems);
                    yield return ConstructSearchQuery(orderedStems, new string[] { }, 1, mode,
                        exactSearchStemWordIndexes);
                }

                yield break;
            }

            // general search case
            // token = 'диктатура'
            // stem = 'диктатур'
            var stemTokens = queryText.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToDictionary(s => _stemmer.Stemm(s).ToLower(), s => s);

            var stemWordIndexes = stemTokens.ToDictionary(st => st.Key, st => new List<uint>());
            for (uint wordIndex = 0; wordIndex < dictionary.Length; wordIndex++)
            {
                foreach (var stem in stemWordIndexes.Keys)
                {
                    if (dictionary[wordIndex].ToLower().StartsWith(stem))
                    {
                        stemWordIndexes[stem].Add(wordIndex);
                    }
                }
            }

            var minTokenCombinationLength = stemTokens.Count - OmitTokensMaxCount;
            if (minTokenCombinationLength < 1)
            {
                minTokenCombinationLength = 1;
            }
            var stemCombinations = SubSetsOf(stemTokens.Keys)
                .Select(ss => ss.ToArray())
                .Where(c => c.Length >= minTokenCombinationLength)
                .ToArray();

            foreach (var stems in stemCombinations)
            {
                var basePriority = (ushort)(1 + (stemTokens.Count - stems.Length) * 20);
                var searchQueries = ConstructSearchQueries(stems, basePriority, stemWordIndexes, mode);
                foreach (var searchQuery in searchQueries)
                {
                    searchQuery.MissingTokens = stemTokens.Keys.Except(stems).Select(s => stemTokens[s]).ToArray();
                    yield return searchQuery;
                }
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/999182/6483508
        /// </summary>
        private static IEnumerable<IEnumerable<T>> SubSetsOf<T>(IEnumerable<T> source)
        {
            if (!source.Any())
                return Enumerable.Repeat(Enumerable.Empty<T>(), 1);

            var element = source.Take(1);

            var haveNots = SubSetsOf(source.Skip(1));
            var haves = haveNots.Select(set => element.Concat(set));

            return haves.Concat(haveNots);
        }

        private IEnumerable<SearchQuery> ConstructSearchQueries(string[] stems, ushort basePriority, Dictionary<string, List<uint>> stemWordIndexes, SearchMode mode)
        {
            yield return ConstructSearchQuery(stems, new string[] { }, basePriority, mode, stemWordIndexes);

            for (var stemIndex = 1; stemIndex < stems.Length; stemIndex++)
            {
                var orderedStems = stems.Take(stemIndex).ToArray();
                var nonOrderedStems = stems.Skip(stemIndex).ToArray();
                var priority = (ushort)(basePriority + stems.Length - stemIndex);
                yield return ConstructSearchQuery(orderedStems, nonOrderedStems, priority, mode, stemWordIndexes);
            }
        }

        private Dictionary<string, List<uint>> GetStemWordIndexes(string[] dictionary, string[] stems)
        {
            var stemWordIndexes = stems.ToDictionary(s => s, s => new List<uint>());
            for (uint wordIndex = 0; wordIndex < dictionary.Length; wordIndex++)
            {
                foreach (var stem in stemWordIndexes.Keys)
                {
                    if (dictionary[wordIndex].ToLower().StartsWith(stem))
                    {
                        stemWordIndexes[stem].Add(wordIndex);
                    }
                }
            }

            return stemWordIndexes;
        }

        private SearchQuery ConstructSearchQuery(string[] orderedStems, string[] nonOrderedStems, ushort priority, SearchMode mode, Dictionary<string, List<uint>> stemWordIndexes)
        {
            var searchQuery = new SearchQuery
            {
                Mode = mode,
                Priority = priority,
                Text = nonOrderedStems.Any()
                    ? $"{string.Join(' ', orderedStems.Select(s => $"{s}*"))} + {string.Join(' ', nonOrderedStems.Select(s => $"{s}*"))}"
                    : $"{string.Join(' ', orderedStems.Select(s => $"{s}*"))}",
                Ordered = new List<SearchToken>(),
                NonOrdered = new List<SearchToken>()
            };

            for (var i = 0; i < orderedStems.Length; i++)
            {
                var stem = orderedStems[i];
                var searchToken = new SearchToken
                {
                    Text = stem,
                    Order = i,
                    TokenType = SearchTokenType.Ordered,
                    WordIndexes = stemWordIndexes[stem]
                };
                searchQuery.Ordered.Add(searchToken);
            }

            for (var i = 0; i < nonOrderedStems.Length; i++)
            {
                var stem = nonOrderedStems[i];
                var searchToken = new SearchToken
                {
                    Text = stem,
                    Order = 0,
                    TokenType = SearchTokenType.NonOrdered,
                    WordIndexes = stemWordIndexes[stem]
                };
                searchQuery.NonOrdered.Add(searchToken);
            }

            return searchQuery;
        }
    }
}