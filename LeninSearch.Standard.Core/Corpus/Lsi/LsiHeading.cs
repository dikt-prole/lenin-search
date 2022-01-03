using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsiHeading
    {
        public byte Level { get; set; }
        public ushort Index { get; set; }
        public List<uint> WordIndexes { get; set; }
        public string GetText(string[] dictionary)
        {
            var words = WordIndexes.Select(wi => dictionary[wi]).ToList();
            return TextUtil.GetParagraph(words);
        }

        public static LsiHeading FromLsParagraph(LsiParagraph paragraph, byte level)
        {
            var lsHeading = new LsiHeading
            {
                Level = level,
                Index = paragraph.Index,
                WordIndexes = new List<uint>(paragraph.WordIndexes)
            };

            return lsHeading;
        }
    }
}