using System;
using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchToken
    {
        public SearchTokenType TokenType { get; set; }
        public List<uint> WordIndexes { get; set; }
        public int Order { get; set; }

        public static SearchToken NonOrdered(string text, string[] dictionary)
        {
            var strict = !text.EndsWith('*');

            text = text.Replace("*", "").ToLower();

            var indexes = new List<uint>();

            if (strict)
            {
                for (var index = 0; index < dictionary.Length; index++)
                {
                    var candidate = dictionary[index];
                    if (candidate.Equals(text, StringComparison.OrdinalIgnoreCase))
                    {
                        indexes.Add((uint)index);
                    }
                }
            }
            else
            {
                for (var index = 0; index < dictionary.Length; index++)
                {
                    var candidate = dictionary[index];
                    if (candidate.StartsWith(text, StringComparison.OrdinalIgnoreCase))
                    {
                        indexes.Add((uint)index);
                    }
                }
            }

            return new SearchToken
            {
                WordIndexes = indexes,
                TokenType = SearchTokenType.NonOrdered
            };
        }

        public static SearchToken Ordered(string text, string[] dictionary, int order)
        {
            var token = NonOrdered(text, dictionary);

            token.Order = order;
            token.TokenType = SearchTokenType.Ordered;

            return token;
        }
    }

    public enum SearchTokenType { Ordered, NonOrdered }
}