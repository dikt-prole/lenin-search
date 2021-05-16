using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Oprimized;

namespace LeninSearch.Standard.Core
{
    public static class LsIndexUtil
    {
        public static byte[] ToLsIndexBytes(OptimizedFileData ofd)
        {
            // construct word position dictionary
            var wordPositionDictionary = new Dictionary<uint, List<WordPosition>>();
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
                    var wordPosition = new WordPosition
                    {
                        ParagraphIndex = (ushort)paragraphIndex,
                        Position = i
                    };

                    if (!wordPositionDictionary.ContainsKey(i))
                    {
                        wordPositionDictionary.Add(i, new List<WordPosition>());
                    }

                    wordPositionDictionary[i].Add(wordPosition);
                }
            }

            // construct word position bytes
            var wordPositionBytes = new List<byte>();
            foreach (var key in wordPositionDictionary.Keys)
            {
                var positionCount = (uint)wordPositionDictionary.Count;
                wordPositionBytes.AddRange(BitConverter.GetBytes(key));
                wordPositionBytes.AddRange(BitConverter.GetBytes(positionCount));
                foreach (var wordPosition in wordPositionDictionary[key])
                {
                    wordPositionBytes.AddRange(BitConverter.GetBytes(wordPosition.ParagraphIndex));
                    wordPositionBytes.AddRange(BitConverter.GetBytes(wordPosition.Position));
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

            var lsBytes = new List<byte>();

            lsBytes.AddRange(BitConverter.GetBytes((ushort)wordPositionBytes.Count)); // word position count
            lsBytes.AddRange(BitConverter.GetBytes((ushort)headerBytes.Count)); // header bytes count
            lsBytes.AddRange(BitConverter.GetBytes((ushort)(pageBytes.Count))); // page bytes count

            lsBytes.AddRange(wordPositionBytes);
            lsBytes.AddRange(headerBytes);
            lsBytes.AddRange(pageBytes);

            return lsBytes.ToArray();
        }

        public class WordPosition
        {
            public ushort ParagraphIndex { get; set; }
            public ushort Position { get; set; }
        }
    }
}
