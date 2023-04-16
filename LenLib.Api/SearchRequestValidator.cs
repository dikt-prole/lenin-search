using System;
using LenLib.Standard.Core.Api;

namespace LenLib.Api
{
    public class SearchRequestValidator
    {
        public string Validate(SearchRequest request)
        {
            var query = request.Query;

            if (string.IsNullOrEmpty(query)) return "Query should not be empty";

            var tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length == 0)
                return "Query should have 1 or more tokens";

            if (tokens.Length > 8)
                return "Query should have less than 8 tokens";

            foreach (var t in tokens)
            {
                var token = t.TrimEnd('*');

                if (token.Length > 32)
                    return "Max length for a token is 32";
            }

            return null;
        }
    }
}