using System;
using System.Collections.Generic;
using System.Linq;

namespace LeninSearch.Standard.Core.OldShit
{
    public class OptimizedHeading
    {
        private readonly List<uint> _localDictionary;
        private bool _isTextReconstructed;
        private string _text;
        public ushort Index { get; set; }
        public byte Level { get; set; }
        public List<ushort> LocalWordIndexes { get; }

        public OptimizedHeading(ushort index, byte level, byte[] bytes, List<uint> localDictionary)
        {
            _localDictionary = localDictionary;
            Index = index;
            Level = level;
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

    public static class OptimizedHeadingExtensions
    {
        public static string GetText(this List<OptimizedHeading> headings, string[] dictionary)
        {
            if (headings == null || headings.Count == 0) return null;

            var headingTexts = headings.OrderBy(h => h.Level).Select(h => h.GetText(dictionary)).ToList();

            return string.Join(" - ", headingTexts);
        }
    }
}