using System.Collections.Generic;

namespace LeninSearch.Standard.Core.Search
{
    public class WordIndexChain
    {
        public WordIndexChain() { }
        public WordIndexChain(uint wordIndex)
        {
            WordIndexes = new List<uint> { wordIndex };
        }

        public List<uint> WordIndexes { get; set; }
        public WordIndexChain Copy(WordIndexChain chain)
        {
            return new WordIndexChain
            {
                WordIndexes = new List<uint>(chain.WordIndexes)
            };
        }
    }
}