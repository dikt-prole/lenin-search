using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search.TokenVarying
{
    public class BruteTokenVariantProvider : ITokenVariantProvider
    {
        private readonly ushort _minTokenLength;
        private readonly ushort[] _omits;

        public BruteTokenVariantProvider(ushort minTokenLength, ushort[] omits)
        {
            _minTokenLength = minTokenLength;
            _omits = omits;
        }

        public IEnumerable<(string Token, ushort Omit)> Vary(string token)
        {
            yield return (token, 0);

            var tokenLength = token.Length;
            var allowedOmits = _omits.Where(o => token.Length - o >= _minTokenLength);
            foreach (var omit in allowedOmits)
            {
                yield return ($"{token.Substring(0, tokenLength - omit)}*", omit);
            }
        }
    }
}