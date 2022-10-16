using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search.TokenVarying
{
    public interface ITokenVariantProvider
    {
        IEnumerable<(string Token, ushort Omit)> Vary(string token);
    }
}