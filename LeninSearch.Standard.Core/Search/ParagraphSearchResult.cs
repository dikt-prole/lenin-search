using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.Search
{
    public class ParagraphSearchResult
    {
        public ParagraphSearchResult(ushort paragraphIndex)
        {
            ParagraphIndex = paragraphIndex;
            WordIndexChains = new List<WordIndexChain>();
        }

        public ParagraphSearchResult(ushort paragraphIndex, List<WordData> wordDatas)
        {
            ParagraphIndex = paragraphIndex;
            WordIndexChains = new List<WordIndexChain>
            {
                new WordIndexChain
                {
                    WordIndexes = wordDatas.Select(w => w.WordIndex).ToList()
                }
            };
        }

        public void AddChain(WordIndexChain chain)
        {
            WordIndexChains.Add(chain);
        }

        public ushort ParagraphIndex { get; set; }
        public List<WordIndexChain> WordIndexChains { get; set; }
        public string File { get; set; }
        public string Text { get; set; }
    }
}