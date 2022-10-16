using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search.TokenVarying
{
    public class EndingRemovingTokenVariantProvider : ITokenVariantProvider
    {
        private readonly ushort _minTokenLength;

        // https://obrazovaka.ru/russkiy-yazyk/tablica-okonchanie-v-russkom-yazyke-3-klass.html
        private static readonly string[] EndingsRaw =
        {
            // существительные
            "и", "ы", "а", "я", "ов", "ей", "е", "ам", "ям", "у", "ю", "ой", "ами", "ями", "ом", "ем", "ах", "ях",

            // прилагательные, причастия
            "ой", "ий", "ый", "ая", "яя", "ое", "ее", "ие", "ые", "ого", "его", "ей", "их", "ых", "ому", "ему", "им", "ым", "ую", "юю", "ими", "ыми", "ом", "ем",

            // глаголы
            "у", "ю", "ем", "им", "ешь", "ете", "ишь", "ите", "ет", "ут", "ют", "ит", "ат", "ят"
        };

        private static readonly string[] Endings = EndingsRaw.Distinct().OrderByDescending(s => s.Length).ToArray();


        public EndingRemovingTokenVariantProvider(ushort minTokenLength)
        {
            _minTokenLength = minTokenLength;
        }

        public IEnumerable<(string Token, ushort Omit)> Vary(string token)
        {
            yield return (token, 0);

            var ending = Endings.FirstOrDefault(end => token.Length - end.Length >= _minTokenLength && token.EndsWith(end));

            if (ending != null)
            {
                yield return ($"{token[..^ending.Length]}*", (ushort)ending.Length);
            }
        }
    }
}