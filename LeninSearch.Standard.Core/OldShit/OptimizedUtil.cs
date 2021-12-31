using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Corpus.Lsi;

namespace LeninSearch.Standard.Core.OldShit
{
    public static class OptimizedUtil
    {
        public static byte[] ToLsIndexBytes(OptimizedFileData ofd)
        {
            // construct word position dictionary
            var wordPositionDictionary = new Dictionary<uint, List<LsWordParagraphData>>();
            var paragraphs = ofd.Paragraphs.ToList();
            var headings = ofd.Headings.ToList();
            var pages = ofd.Pages;

            var localDictionary = ofd.InversedLocalDictionary.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            for (var paragraphIndex = 0; paragraphIndex < paragraphs.Count; paragraphIndex++)
            {
                var heading = headings.FirstOrDefault(h => h.Index == paragraphIndex);
                var paragraph = paragraphs[paragraphIndex];
                var words = heading == null
                    ? paragraph.LocalWordIndexes.Select(i => localDictionary[i]).ToList()
                    : heading.LocalWordIndexes.Select(i => localDictionary[i]).ToList();

                if (words.Count == 0) continue;

                for (ushort i = 0; i < words.Count; i++)
                {
                    var wordPosition = new LsWordParagraphData
                    {
                        ParagraphIndex = (ushort)paragraphIndex,
                        WordPosition = i
                    };

                    if (!wordPositionDictionary.ContainsKey(words[i]))
                    {
                        wordPositionDictionary.Add(words[i], new List<LsWordParagraphData>());
                    }

                    wordPositionDictionary[words[i]].Add(wordPosition);
                }
            }

            // construct word position bytes
            var wordPositionBytes = new List<byte>();
            foreach (var key in wordPositionDictionary.Keys)
            {
                var positionCount = (uint)wordPositionDictionary[key].Count;
                wordPositionBytes.AddRange(BitConverter.GetBytes(key));
                wordPositionBytes.AddRange(BitConverter.GetBytes(positionCount));
                foreach (var wordPosition in wordPositionDictionary[key])
                {
                    wordPositionBytes.AddRange(BitConverter.GetBytes(wordPosition.ParagraphIndex));
                    wordPositionBytes.AddRange(BitConverter.GetBytes(wordPosition.WordPosition));
                }
            }

            // construct header bytes
            var headerBytes = new List<byte>();
            if (headings.Any())
            {
                foreach (var h in headings)
                {
                    headerBytes.AddRange(BitConverter.GetBytes(h.Index));
                    headerBytes.Add(h.Level);
                }
            }

            // construct page bytes
            var pageBytes = new List<byte>();
            if (pages?.Any() == true)
            {
                foreach (var p in pages)
                {
                    pageBytes.AddRange(BitConverter.GetBytes(p.Key));
                    pageBytes.AddRange(BitConverter.GetBytes(p.Value));
                }
            }

            var lsiBytes = new List<byte>();

            lsiBytes.AddRange(BitConverter.GetBytes((uint)wordPositionBytes.Count)); // word position count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)headerBytes.Count)); // header bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)(pageBytes.Count))); // page bytes count

            lsiBytes.AddRange(wordPositionBytes);
            lsiBytes.AddRange(headerBytes);
            lsiBytes.AddRange(pageBytes);

            return lsiBytes.ToArray();
        }
    }
}