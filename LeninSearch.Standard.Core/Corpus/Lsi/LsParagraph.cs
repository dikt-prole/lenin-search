using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Corpus.Lsi
{
    public class LsParagraph
    {
        private string _text = null;
        public LsParagraph(ushort index)
        {
            Index = index;
            WordIndexes = new List<uint>();
        }

        public ushort Index { get; set; }
        public List<uint> WordIndexes { get; set; }
        public string GetText(string[] dictionary)
        {
            if (string.IsNullOrEmpty(_text))
            {
                var words = WordIndexes.Select(wi => dictionary[wi]).ToList();
                _text = TextUtil.GetParagraph(words);
            }

            return _text;
        }

        // additional
        public bool IsPageNumber { get; set; }
        public ushort PageNumber { get; set; }
        public bool IsHeading { get; set; }
    }
}