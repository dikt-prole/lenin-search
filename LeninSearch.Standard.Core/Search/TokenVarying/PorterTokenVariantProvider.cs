using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search.TokenVarying
{
    public class PorterTokenVariantProvider : ITokenVariantProvider
    {
        public IEnumerable<(string Token, ushort Omit)> Vary(string token)
        {
            var porterResult = Porter.TransformingWord(token);
            yield return ($"{porterResult}*", (ushort)(token.Length - porterResult.Length));
        }
    }
}