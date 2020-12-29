using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Xam.Core.Oprimized
{
    public class OptimizedParagraph
    {
        private readonly List<uint> _localDictionary;
        public ushort Index { get; set; }
        public List<ushort> LocalWordIndexes { get; }
        public bool IsPageNumber { get; set; }
        public ushort PageNumber { get; set; }
        public bool IsHeader { get; set; }

        public OptimizedParagraph(byte[] bytes, List<uint> localDictionary)
        {
            _localDictionary = localDictionary;
            LocalWordIndexes = new List<ushort>();
            for (var i = 0; i < bytes.Length; i += 2)
            {
                LocalWordIndexes.Add(BitConverter.ToUInt16(bytes, i));
            }
        }

        public string GetText()
        {
            var words = GetWords().ToList();

            return TextUtil.GetParagraph(words);
        }

        public IEnumerable<string> GetWords()
        {
            return LocalWordIndexes.Select(i => OptimizedDictionary.Instance[_localDictionary[i]]);
        }
    }
}