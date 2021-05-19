using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.OldShit
{
    public class OptimizedParagraph
    {
        private readonly List<uint> _localDictionary;
        private bool _isTextReconstructed;
        private string _text;
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

        public string GetText(string[] dictionary)
        {
            if (_isTextReconstructed) return _text;

            var words = LocalWordIndexes.Select(i => dictionary[_localDictionary[i]]).ToList();

            _text = TextUtil.GetParagraph(words);

            _isTextReconstructed = true;

            return _text;
        }
    }
}