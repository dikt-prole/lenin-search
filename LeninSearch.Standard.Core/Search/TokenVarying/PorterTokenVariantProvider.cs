using System.Collections.Generic;
using System.Diagnostics;

namespace LeninSearch.Standard.Core.Search.TokenVarying
{
    public class PorterTokenVariantProvider : ITokenVariantProvider
    {
        public IEnumerable<(string Token, ushort Omit)> Vary(string token)
        {
            var porterResult = new RuPorter().Stemm(token);
            Debug.WriteLine($"Porter: in='{token}', out={porterResult}");
            yield return ($"{porterResult}*", (ushort)(token.Length - porterResult.Length));
        }
    }
}