using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class SearchQuery
    {
        public string Text { get; set; }
        public List<SearchToken> Ordered { get; set; }
        public List<SearchToken> NonOrdered { get; set; }
        public SearchQueryType QueryType { get; set; }
        public ushort Priority { get; set; }
        public bool IsHeading => QueryType == SearchQueryType.Heading;

        public static SearchQuery Construct(string text, string[] dictionary)
        {
            var request = new SearchQuery
            {
                Text = text,
                Ordered = new List<SearchToken>(),
                NonOrdered = new List<SearchToken>(),
                QueryType = text.StartsWith('*') ? SearchQueryType.Heading : SearchQueryType.Paragraph
            };

            text = text.TrimStart('*', ' ');

            var plusIndex = text.IndexOf('+');
            if (plusIndex < 0)
            {
                var tokenTexts = GetTokenTexts(text).ToList();
                for (var i = 0; i < tokenTexts.Count; i++)
                {
                    var tokenText = tokenTexts[i];
                    var searchToken = SearchToken.Ordered(tokenText, dictionary, i);
                    request.Ordered.Add(searchToken);
                }
            }
            else
            {
                var beforePlus = text.Substring(0, plusIndex);
                var afterPlus = text.Substring(plusIndex + 1);

                var beforePlusTokenTexts = GetTokenTexts(beforePlus).ToList();
                for (var i = 0; i < beforePlusTokenTexts.Count; i++)
                {
                    var tokenText = beforePlusTokenTexts[i];
                    var searchToken = SearchToken.Ordered(tokenText, dictionary, i);
                    request.Ordered.Add(searchToken);
                }

                var afterPlusTokenTexts = GetTokenTexts(afterPlus).ToList();
                for (var i = 0; i < afterPlusTokenTexts.Count; i++)
                {
                    var tokenText = afterPlusTokenTexts[i];
                    var searchToken = SearchToken.NonOrdered(tokenText, dictionary);
                    request.NonOrdered.Add(searchToken);
                }
            }

            return request;
        }

        private static IEnumerable<string> GetTokenTexts(string text)
        {
            var spaceSplit = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < spaceSplit.Length; i++)
            {
                var splitUnit = new string(spaceSplit[i].Where(c => char.IsLetter(c) || c == '*').ToArray());
                yield return splitUnit;
            }
        }

        public SearchQuery Copy()
        {
            return new SearchQuery
            {
                Text = Text,
                Ordered = Ordered.Select(t => t.Copy()).ToList(),
                NonOrdered = NonOrdered.Select(t => t.Copy()).ToList()
            };
        }
    }
}