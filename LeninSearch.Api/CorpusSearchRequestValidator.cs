using System;
using System.Linq;
using LeninSearch.Standard.Core.Api;

namespace LeninSearch.Api
{
    public class CorpusSearchRequestValidator
    {
        private static char[] AllowedChars = {' ', '+', '*'};

        public string Validate(CorpusSearchRequestNew request)
        {
            var query = request.Query;

            if (string.IsNullOrEmpty(query)) return "Query should not be empty";

            if (query.Any(c => !char.IsLetter(c) && !AllowedChars.Contains(c)))
                return "Query contains not allowed chars";

            if (query.Count(c => c == '+') > 1)
                return "Only one '+' for a query allowed";

            query = query.Replace('+', ' ').TrimStart(' ', '*');

            var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
                return "Query should have 1 or more tokens";

            if (tokens.Length > 8)
                return "Query should have less than 8 tokens";

            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i].TrimEnd('*');

                if (token.Length == 0)
                    return "Empty search tokens are not allowed";

                if (token.Length > 16)
                    return "Max length for a token is 16";
            }

            return null;
        }
    }
}