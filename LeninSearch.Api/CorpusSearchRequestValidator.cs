using System;
using System.Linq;
using LeninSearch.Standard.Core.Api;

namespace LeninSearch.Api
{
    public class CorpusSearchRequestValidator
    {
        public string Validate(CorpusSearchRequest request)
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