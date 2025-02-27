﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LenLib.Standard.Core.Corpus.Json;
using LenLib.Standard.Core.Corpus.Lsi;

namespace LenLib.Standard.Core.LsiUtil
{
    public class V2LsiUtil : ILsiUtil
    {
        public const byte LsiVersion = 2;
        public const int FileHeaderLength = 64;
        public byte[] ToLsIndexBytes(JsonFileData fd, Dictionary<string, uint> reverseDictionary)
        {
            // construct word position dictionary
            var wordPositionDictionary = new Dictionary<uint, List<LsWordParagraphData>>();
            var paragraphs = fd.Pars.ToList();
            var headings = fd.Headings?.ToList() ?? new List<JsonHeading>();
            var pages = fd.Pages?.ToList() ?? new List<KeyValuePair<ushort, ushort>>();
            var offsets = new List<KeyValuePair<ushort, ushort>>();
            var videoData = new List<LsiVideoDataItem>();
            var markupData = new Dictionary<ushort, List<LsiMarkupData>>();
            var commentData = new Dictionary<ushort, List<LsiCommentData>>();
            var imageData = new Dictionary<ushort, ushort>();
            var inlineImages = new Dictionary<ushort, List<LsiInlineImageData>>();

            for (ushort paragraphIndex = 0; paragraphIndex < paragraphs.Count; paragraphIndex++)
            {
                var heading = headings.FirstOrDefault(h => h.Index == paragraphIndex);
                var paragraph = paragraphs[paragraphIndex];

                var words = heading == null
                    ? TextUtil.GetOrderedWords(paragraph.Text)
                    : TextUtil.GetOrderedWords(heading.Text);

                // processing markup
                foreach (var markup in paragraph.Markups ?? new List<JsonMarkupData>())
                {
                    var beforeMarkup = paragraph.Text.Substring(0, markup.MarkupSymbolStart);
                    var beforeMarkupWords = TextUtil.GetOrderedWords(beforeMarkup);
                    var markupText = paragraph.Text.Substring(markup.MarkupSymbolStart, markup.MarkupSymbolLength);
                    var markupWords = TextUtil.GetOrderedWords(markupText);
                    if (!markupData.ContainsKey(paragraphIndex)) markupData.Add(paragraphIndex, new List<LsiMarkupData>());
                    markupData[paragraphIndex].Add(new LsiMarkupData
                    {
                        MarkupType = markup.MarkupType,
                        WordLength = (ushort)markupWords.Count,
                        WordPosition = (ushort)beforeMarkupWords.Count
                    });
                }

                // processing comments
                foreach (var comment in paragraph.Comments ?? new List<JsonCommentData>())
                {
                    var beforeCommentText = paragraph.Text.Substring(0, comment.CommentSymbolStart);
                    var beforeCommentWords = TextUtil.GetOrderedWords(beforeCommentText);
                    var commentWords = TextUtil.GetOrderedWords(comment.Text);
                    var commentWordIndexes = commentWords.Select(w => reverseDictionary[w]).ToList();
                    if (!commentData.ContainsKey(paragraphIndex)) commentData.Add(paragraphIndex, new List<LsiCommentData>());
                    commentData[paragraphIndex].Add(new LsiCommentData
                    {
                        WordPosition = (ushort)beforeCommentWords.Count,
                        CommentIndex = comment.CommentIndex,
                        WordIndexes = commentWordIndexes.ToArray()
                    });
                }

                // processing inline images
                foreach (var jsonInlineImage in paragraph.InlineImages ?? new List<JsonInlineImageData>())
                {
                    var beforeImageText = paragraph.Text.Substring(0, jsonInlineImage.ImageSymbolStart);
                    var beforeImageWords = TextUtil.GetOrderedWords(beforeImageText);
                    var imageIndex = jsonInlineImage.ImageIndex;
                    if (!inlineImages.ContainsKey(paragraphIndex)) inlineImages.Add(paragraphIndex, new List<LsiInlineImageData>());
                    inlineImages[paragraphIndex].Add(new LsiInlineImageData
                    {
                        ImageIndex = imageIndex,
                        WordPosition = (ushort)beforeImageWords.Count
                    });
                }

                // processing images
                if (paragraph.ImageIndex.HasValue)
                {
                    imageData.Add(paragraphIndex, paragraph.ImageIndex.Value);
                }

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

            // construct image bytes
            var imageBytes = new List<byte>();
            foreach (ushort paragraphIndex in imageData.Keys)
            {
                imageBytes.AddRange(BitConverter.GetBytes(paragraphIndex));
                imageBytes.AddRange(BitConverter.GetBytes(imageData[paragraphIndex]));
            }

            if (imageBytes.Count >= ushort.MaxValue) throw new Exception("Too many images!");

            // construct markup bytes
            var markupBytes = new List<byte>();
            foreach (ushort paragraphIndex in markupData.Keys)
            {
                foreach (var markup in markupData[paragraphIndex])
                {
                    markupBytes.AddRange(BitConverter.GetBytes(paragraphIndex));
                    markupBytes.AddRange(BitConverter.GetBytes(markup.WordPosition));
                    markupBytes.AddRange(BitConverter.GetBytes(markup.WordLength));
                    markupBytes.Add((byte)markup.MarkupType);
                }
            }

            // construct comment bytes
            var commentBytes = new List<byte>();
            foreach (ushort paragraphIndex in commentData.Keys)
            {
                foreach (var comment in commentData[paragraphIndex])
                {
                    commentBytes.AddRange(BitConverter.GetBytes(paragraphIndex));
                    commentBytes.AddRange(BitConverter.GetBytes(comment.CommentIndex));
                    commentBytes.AddRange(BitConverter.GetBytes(comment.WordPosition));
                    commentBytes.AddRange(BitConverter.GetBytes((ushort)comment.WordIndexes.Length));
                    foreach (var word in comment.WordIndexes)
                    {
                        commentBytes.AddRange(BitConverter.GetBytes(word));
                    }
                }
            }

            // construct inline image bytes
            var inlineImageBytes = new List<byte>();
            foreach (ushort paragraphIndex in inlineImages.Keys)
            {
                foreach (var inlineImage in inlineImages[paragraphIndex])
                {
                    inlineImageBytes.AddRange(BitConverter.GetBytes(paragraphIndex));
                    inlineImageBytes.AddRange(BitConverter.GetBytes(inlineImage.ImageIndex));
                    inlineImageBytes.AddRange(BitConverter.GetBytes(inlineImage.WordPosition));
                }
            }

            var lsiBytes = new List<byte>();

            lsiBytes.Add(LsiVersion);
            lsiBytes.AddRange(BitConverter.GetBytes((uint)wordPositionBytes.Count)); // word position count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)headerBytes.Count)); // header bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)pageBytes.Count)); // page bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)videoBytes.Count)); // video bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)offsetBytes.Count)); // offset bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)imageBytes.Count)); // image bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)markupBytes.Count)); // markup bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)commentBytes.Count)); // comment bytes count
            lsiBytes.AddRange(BitConverter.GetBytes((uint)inlineImageBytes.Count)); // inline image bytes count
            while (lsiBytes.Count < FileHeaderLength)
            {
                lsiBytes.Add(0);
            }

            lsiBytes.AddRange(wordPositionBytes);
            lsiBytes.AddRange(headerBytes);
            lsiBytes.AddRange(pageBytes);
            lsiBytes.AddRange(videoBytes);
            lsiBytes.AddRange(offsetBytes);
            lsiBytes.AddRange(imageBytes);
            lsiBytes.AddRange(markupBytes);
            lsiBytes.AddRange(commentBytes);
            lsiBytes.AddRange(inlineImageBytes);

            return lsiBytes.ToArray();
        }

        public LsiData FromLsIndexBytes(byte[] lsIndexBytes)
        {
            var cursor = 0;

            var lsiVersion = lsIndexBytes[cursor]; cursor++;
            var wordPositionBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var headingBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var pageBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var videoBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var offsetBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var imageBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var markupBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var commentBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
            var inlineImageBytesCount = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;

            cursor = FileHeaderLength;

            var lsIndexData = new LsiData
            {
                WordParagraphData = new Dictionary<uint, List<LsWordParagraphData>>(),
                Headings = new List<LsWordHeadingData>(),
                Pages = new List<LsPageData>(),
                Offsets = new Dictionary<ushort, ushort>(),
                Videos = new List<LsiVideoDataItem>(),
                Images = new Dictionary<ushort, ushort>(),
                Markups = new Dictionary<ushort, List<LsiMarkupData>>(),
                Comments = new Dictionary<ushort, List<LsiCommentData>>(),
                InlineImages = new Dictionary<ushort, List<LsiInlineImageData>>()
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

            // 6. read images
            upperMargin += imageBytesCount;
            while (cursor < upperMargin)
            {
                var paragraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var imageIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                lsIndexData.Images.Add(paragraphIndex, imageIndex);
            }

            // 7. read markups
            upperMargin += markupBytesCount;
            while (cursor < upperMargin)
            {
                var paragraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var wordIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var length = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var markupType = (MarkupType)lsIndexBytes[cursor]; cursor += 1;
                if (!lsIndexData.Markups.ContainsKey(paragraphIndex))
                {
                    lsIndexData.Markups.Add(paragraphIndex, new List<LsiMarkupData>());
                }
                lsIndexData.Markups[paragraphIndex].Add(new LsiMarkupData
                {
                    WordPosition = wordIndex,
                    WordLength = length,
                    MarkupType = markupType
                });
            }

            // 8. read comments
            upperMargin += commentBytesCount;
            while (cursor < upperMargin)
            {
                var paragraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var commentIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var wordIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var length = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var words = new List<uint>();
                for (var i = 0; i < length; i++)
                {
                    var word = BitConverter.ToUInt32(lsIndexBytes, cursor); cursor += 4;
                    words.Add(word);
                }
                if (!lsIndexData.Comments.ContainsKey(paragraphIndex))
                {
                    lsIndexData.Comments.Add(paragraphIndex, new List<LsiCommentData>());
                }
                lsIndexData.Comments[paragraphIndex].Add(new LsiCommentData
                {
                    CommentIndex = commentIndex,
                    WordPosition = wordIndex,
                    WordIndexes = words.ToArray()
                });
            }

            // 9. read inline images
            upperMargin += inlineImageBytesCount;
            while (cursor < upperMargin)
            {
                var paragraphIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var imageIndex = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                var wordPosition = BitConverter.ToUInt16(lsIndexBytes, cursor); cursor += 2;
                if (!lsIndexData.InlineImages.ContainsKey(paragraphIndex))
                {
                    lsIndexData.InlineImages.Add(paragraphIndex, new List<LsiInlineImageData>());
                }
                lsIndexData.InlineImages[paragraphIndex].Add(new LsiInlineImageData
                {
                    ImageIndex = imageIndex,
                    WordPosition = wordPosition
                });
            }

            return lsIndexData;
        }
    }
}