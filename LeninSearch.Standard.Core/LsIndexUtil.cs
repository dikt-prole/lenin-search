using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.OldShit;
using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Standard.Core
{
    public static class LsIndexUtil
    {
        public const byte LsiVersion = 1;
        public const int FileHeaderLength = 32;

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

        public static byte[] ToLsIndexBytes(FileData fd, Dictionary<string, uint> globalWords)
        {
            // construct word position dictionary
            var wordPositionDictionary = new Dictionary<uint, List<LsWordParagraphData>>();
            var paragraphs = fd.Pars.ToList();
            var headings = fd.Headings?.ToList() ?? new List<Heading>();
            var pages = fd.Pages?.ToList() ?? new List<KeyValuePair<ushort, ushort>>();
            var offsets = new List<KeyValuePair<ushort, ushort>>();

            for (var paragraphIndex = 0; paragraphIndex < paragraphs.Count; paragraphIndex++)
            {
                var heading = headings.FirstOrDefault(h => h.Index == paragraphIndex);
                var paragraph = paragraphs[paragraphIndex];
                var words = heading == null
                    ? TextUtil.GetOrderedWords(paragraph.Text).Select(w => globalWords[w]).ToList()
                    : TextUtil.GetOrderedWords(heading.Text).Select(w => globalWords[w]).ToList();

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

                if (paragraph.ParagraphType == ParagraphType.Youtube)
                {
                    offsets.Add(new KeyValuePair<ushort, ushort>((ushort)paragraphIndex, paragraph.OffsetSeconds));
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

            // construct offset bytes
            var offsetBytes = new List<byte>();
            foreach (var offset in offsets)
            {
                offsetBytes.AddRange(BitConverter.GetBytes(offset.Key));
                offsetBytes.AddRange(BitConverter.GetBytes(offset.Value));
            }


            var lsiBytes = new List<byte>();

            lsiBytes.Add(LsiVersion);
            lsiBytes.AddRange(BitConverter.GetBytes((uint)wordPositionBytes.Count)); // word position count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)headerBytes.Count)); // header bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)(pageBytes.Count))); // page bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)(offsetBytes.Count))); // offset bytes count
            while (lsiBytes.Count < FileHeaderLength)
            {
                lsiBytes.Add(0);
            }

            lsiBytes.AddRange(wordPositionBytes);
            lsiBytes.AddRange(headerBytes);
            lsiBytes.AddRange(pageBytes);
            lsiBytes.AddRange(offsetBytes);

            return lsiBytes.ToArray();
        }

        public static LsIndexData FromLsIndexBytes(byte[] lsIndexBytes)
        {
            var cursor = 0;

            var lsiVersion = lsIndexBytes[cursor]; cursor++;
            var wordPositionBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var headingBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var pageBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var offsetBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;

            cursor = FileHeaderLength;

            var lsIndexData = new LsIndexData
            {
                WordParagraphData = new Dictionary<uint, List<LsWordParagraphData>>(),
                HeadingData = new List<LsWordHeadingData>(),
                PageData = new List<LsPageData>(),
                VideoOffsets = new Dictionary<ushort, ushort>()
            };

            // 1. read word positions
            var upperMargin = FileHeaderLength + wordPositionBytesCount;
            while (cursor < upperMargin)
            {
                var wordIndex = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
                var positionCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
                var wordInParagraphPositions = new List<LsWordParagraphData>();
                for (var i = 0; i < positionCount; i++)
                {
                    var paragraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                    var wordPosition = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                    wordInParagraphPositions.Add(new LsWordParagraphData
                    {
                        ParagraphIndex = paragraphIndex,
                        WordPosition = wordPosition
                    });
                }
                lsIndexData.WordParagraphData.Add(wordIndex, wordInParagraphPositions);
            }

            // 2. read headings
            upperMargin += headingBytesCount;
            while (cursor < upperMargin)
            {
                var headingIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var headingLevel = lsIndexBytes[cursor]; cursor += 1;
                var headingData = new LsWordHeadingData
                {
                    Index = headingIndex,
                    Level = headingLevel
                };
                lsIndexData.HeadingData.Add(headingData);
            }

            // 3. read pages
            upperMargin += pageBytesCount;
            while (cursor < upperMargin)
            {
                var pageIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var pageNumber = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var pageData = new LsPageData
                {
                    Index = pageIndex,
                    Number = pageNumber
                };
                lsIndexData.PageData.Add(pageData);
            }

            // 4. read offsets
            upperMargin += offsetBytesCount;
            while (cursor < upperMargin)
            {
                var offsetIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var offsetValue = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                lsIndexData.VideoOffsets.Add(offsetIndex, offsetValue);
            }

            return lsIndexData;
        }
    }
}
