using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.Corpus.Lsi;

namespace LeninSearch.Standard.Core.LsiUtil
{
    public class V1LsiUtil
    {
        public const byte LsiVersion = 1;
        public const int FileHeaderLength = 32;
        public byte[] ToLsIndexBytes(JsonFileData fd, Dictionary<string, uint> reverseDictionary)
        {
            // construct word position dictionary
            var wordPositionDictionary = new Dictionary<uint, List<LsWordParagraphData>>();
            var paragraphs = fd.Pars.ToList();
            var headings = fd.Headings?.ToList() ?? new List<JsonHeading>();
            var pages = fd.Pages?.ToList() ?? new List<KeyValuePair<ushort, ushort>>();
            var offsets = new List<KeyValuePair<ushort, ushort>>();
            var videoData = new List<LsiVideoDataItem>();

            for (ushort paragraphIndex = 0; paragraphIndex < paragraphs.Count; paragraphIndex++)
            {
                var heading = headings.FirstOrDefault(h => h.Index == paragraphIndex);
                var paragraph = paragraphs[paragraphIndex];

                var words = heading == null
                    ? TextUtil.GetOrderedWords(paragraph.Text)
                    : TextUtil.GetOrderedWords(heading.Text);

                var wordIndexes = words.Select(w => reverseDictionary[w]).ToList();

                if (wordIndexes.Count == 0) continue;

                for (ushort i = 0; i < wordIndexes.Count; i++)
                {
                    var wordPosition = new LsWordParagraphData
                    {
                        ParagraphIndex = paragraphIndex,
                        WordPosition = i
                    };

                    if (!wordPositionDictionary.ContainsKey(wordIndexes[i]))
                    {
                        wordPositionDictionary.Add(wordIndexes[i], new List<LsWordParagraphData>());
                    }

                    wordPositionDictionary[wordIndexes[i]].Add(wordPosition);
                }

                if (paragraph.ParagraphType == JsonParagraphType.Youtube)
                {
                    var lastVideoItem = videoData.LastOrDefault();
                    if (lastVideoItem != null && lastVideoItem.VideoId == paragraph.VideoId && paragraphIndex - lastVideoItem.LastParagraphIndex == 1)
                    {
                        lastVideoItem.LastParagraphIndex = paragraphIndex;
                    }
                    else
                    {
                        var videoItem = new LsiVideoDataItem(paragraph.VideoId, paragraphIndex, paragraphIndex);
                        videoData.Add(videoItem);
                    }

                    offsets.Add(new KeyValuePair<ushort, ushort>(paragraphIndex, paragraph.OffsetSeconds));
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

            // construct video bytes
            var videoBytes = new List<byte>();
            foreach (var videoDataItem in videoData)
            {
                var videoIdBytes = Encoding.UTF8.GetBytes(videoDataItem.VideoId);
                videoBytes.Add((byte)videoIdBytes.Length);
                videoBytes.AddRange(videoIdBytes);
                videoBytes.AddRange(BitConverter.GetBytes(videoDataItem.FirstParagraphIndex));
                videoBytes.AddRange(BitConverter.GetBytes(videoDataItem.LastParagraphIndex));
            }

            var lsiBytes = new List<byte>();

            lsiBytes.Add(LsiVersion);
            lsiBytes.AddRange(BitConverter.GetBytes((uint)wordPositionBytes.Count)); // word position count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)headerBytes.Count)); // header bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)(pageBytes.Count))); // page bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)(videoBytes.Count))); // video bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)(offsetBytes.Count))); // offset bytes count
            while (lsiBytes.Count < FileHeaderLength)
            {
                lsiBytes.Add(0);
            }

            lsiBytes.AddRange(wordPositionBytes);
            lsiBytes.AddRange(headerBytes);
            lsiBytes.AddRange(pageBytes);
            lsiBytes.AddRange(videoBytes);
            lsiBytes.AddRange(offsetBytes);

            return lsiBytes.ToArray();
        }

        public LsIndexData FromLsIndexBytes(byte[] lsIndexBytes)
        {
            var cursor = 0;

            var lsiVersion = lsIndexBytes[cursor]; cursor++;
            var wordPositionBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var headingBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var pageBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var videoBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var offsetBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;

            cursor = FileHeaderLength;

            var lsIndexData = new LsIndexData
            {
                WordParagraphData = new Dictionary<uint, List<LsWordParagraphData>>(),
                Headings = new List<LsWordHeadingData>(),
                Pages = new List<LsPageData>(),
                Offsets = new Dictionary<ushort, ushort>(),
                Videos = new List<LsiVideoDataItem>(),
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
                lsIndexData.Headings.Add(headingData);
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
                lsIndexData.Pages.Add(pageData);
            }

            // 4. read video data
            upperMargin += videoBytesCount;
            while (cursor < upperMargin)
            {
                var videoIdLength = lsIndexBytes[cursor]; cursor++;
                var videoId = Encoding.UTF8.GetString(lsIndexBytes, cursor, videoIdLength); cursor += videoIdLength;
                var firstParagraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var lastParagraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                lsIndexData.Videos.Add(new LsiVideoDataItem(videoId, firstParagraphIndex, lastParagraphIndex));
            }

            // 5. read offsets
            upperMargin += offsetBytesCount;
            while (cursor < upperMargin)
            {
                var offsetIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var offsetValue = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                lsIndexData.Offsets.Add(offsetIndex, offsetValue);
            }

            return lsIndexData;
        }
    }
}