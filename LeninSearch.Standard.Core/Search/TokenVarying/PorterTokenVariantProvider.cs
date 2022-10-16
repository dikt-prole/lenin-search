using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search.TokenVarying
{
    public class PorterTokenVariantProvider : ITokenVariantProvider
    {
        public IEnumerable<(string Token, ushort Omit)> Vary(string token)
        {
            yield return (token, 0);

            var porterResult = Porter.TransformingWord(token);

            if (porterResult.Length < token.Length)
            {
                yield return ($"{porterResult}*", (ushort)(token.Length - porterResult.Length));
            }
        }
    }
}