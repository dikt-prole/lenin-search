using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using LeninSearch.Core.Oprimized;

namespace LeninSearch.Core
{
    public static class ArchiveUtil
    {
        public static byte[] Save(FileData fileData, Dictionary<string, uint> globalDictionary)
        {
            var localDictionary = new List<uint>();

            // construct paragraph bytes
            var paragraphBytes = new List<byte>();
            foreach (var p in fileData.Pars)
            {
                var pWords = TextUtil.GetOrderedWords(p.Text).ToList();
                var paragraphLength = (ushort) pWords.Count;
                paragraphBytes.AddRange(BitConverter.GetBytes(paragraphLength));
                foreach (var w in pWords)
                {
                    uint wordGlobalKey = 0;
                    if (!globalDictionary.ContainsKey(w))
                    {
                        wordGlobalKey = globalDictionary.Values.Count == 0 ? 0 : globalDictionary.Values.Max() + 1;
                        globalDictionary.Add(w, wordGlobalKey);
                    }

                    wordGlobalKey = globalDictionary[w];
                    if (!localDictionary.Contains(wordGlobalKey))
                    {
                        if (localDictionary.Count >= ushort.MaxValue)
                        {
                            throw new Exception("Unable to archive this file (localDictionary.Count > ushort.MaxValue)");
                        }
                        localDictionary.Add(wordGlobalKey);
                    }

                    var wordLocalKey = (ushort)localDictionary.IndexOf(wordGlobalKey);

                    paragraphBytes.AddRange(BitConverter.GetBytes(wordLocalKey));
                }
            }

            // construct page bytes
            var pageBytes = new List<byte>();
            if (fileData.Pages?.Any() == true)
            {
                foreach (var p in fileData.Pages)
                {
                    pageBytes.AddRange(BitConverter.GetBytes(p.Key));
                    pageBytes.AddRange(BitConverter.GetBytes(p.Value));
                }
            }

            // construct header bytes
            var headerBytes = new List<byte>();
            if (fileData.Headings.Any() == true)
            {
                foreach (var h in fileData.Headings)
                {
                    headerBytes.AddRange(BitConverter.GetBytes((ushort)h.Index));
                    var hWords = TextUtil.GetOrderedWords(h.Text);
                    headerBytes.AddRange(BitConverter.GetBytes((ushort)hWords.Count));
                    foreach (var w in hWords)
                    {
                        uint wordGlobalKey = 0;
                        if (!globalDictionary.ContainsKey(w))
                        {
                            wordGlobalKey = globalDictionary.Values.Count == 0 ? 0 : globalDictionary.Values.Max() + 1;
                            globalDictionary.Add(w, wordGlobalKey);
                        }

                        wordGlobalKey = globalDictionary[w];
                        if (!localDictionary.Contains(wordGlobalKey))
                        {
                            if (localDictionary.Count >= ushort.MaxValue)
                            {
                                throw new Exception("Unable to archive this file (localDictionary.Count > ushort.MaxValue)");
                            }
                            localDictionary.Add(wordGlobalKey);
                        }

                        var wordLocalKey = (ushort)localDictionary.IndexOf(wordGlobalKey);

                        headerBytes.AddRange(BitConverter.GetBytes(wordLocalKey));
                    }
                }
            }

            // construct word bytes
            var wordBytes = localDictionary.SelectMany(BitConverter.GetBytes).ToArray();

            var lsBytes = new List<byte>();

            lsBytes.AddRange(BitConverter.GetBytes((ushort)localDictionary.Count)); // word count
            lsBytes.AddRange(BitConverter.GetBytes((ushort)fileData.Pars.Count)); // paragraph count
            lsBytes.AddRange(BitConverter.GetBytes((ushort)(fileData.Pages?.Count ?? 0))); // page count
            lsBytes.AddRange(BitConverter.GetBytes((ushort)(fileData.Headings?.Count ?? 0))); // header count

            lsBytes.AddRange(wordBytes);
            lsBytes.AddRange(paragraphBytes);
            lsBytes.AddRange(pageBytes);
            lsBytes.AddRange(headerBytes);

            return lsBytes.ToArray();
        }

        public static OptimizedFileData LoadOptimized(byte[] lsBytes, CancellationToken ct)
        {
            if (ct.IsCancellationRequested) return null;

            var wordCount = BitConverter.ToUInt16(lsBytes, 0);
            var paragraphCount = BitConverter.ToUInt16(lsBytes, 2);
            var pageCount = BitConverter.ToUInt16(lsBytes, 4);
            var headerCount = BitConverter.ToUInt16(lsBytes, 6);

            int cursor = 8;

            var localDictionary = new List<uint>();
            for (var i = 0; i < wordCount; i++)
            {
                if (ct.IsCancellationRequested) return null;

                var globalIndex = BitConverter.ToUInt32(lsBytes, cursor);
                localDictionary.Add(globalIndex);
                cursor += 4;
            }

            var optimizedFileData = new OptimizedFileData(localDictionary);

            for (ushort paragraphIndex = 0; paragraphIndex < paragraphCount; paragraphIndex++)
            {
                if (ct.IsCancellationRequested) return null;

                var paragraphLength = BitConverter.ToUInt16(lsBytes, cursor);

                cursor += 2;

                var pBytes = lsBytes.Skip(cursor).Take(paragraphLength * 2).ToArray();
                var optimizedParagraph = new OptimizedParagraph(pBytes, localDictionary);
                optimizedFileData.AddParagraph(optimizedParagraph);

                cursor += pBytes.Length;
            }

            if (pageCount > 0)
            {
                for (ushort i = 0; i < pageCount; i++)
                {
                    if (ct.IsCancellationRequested) return null;

                    var pageIndex = BitConverter.ToUInt16(lsBytes, cursor);
                    cursor += 2;
                    var pageNumber = BitConverter.ToUInt16(lsBytes, cursor);
                    cursor += 2;
                    optimizedFileData.AddPage(pageIndex, pageNumber);
                }
            }

            if (headerCount > 0)
            {
                for (ushort i = 0; i < headerCount; i++)
                {
                    if (ct.IsCancellationRequested) return null;

                    var headerIndex = BitConverter.ToUInt16(lsBytes, cursor);
                    cursor += 2;
                    var headerLength = BitConverter.ToUInt16(lsBytes, cursor);
                    cursor += 2;

                    var hBytes = lsBytes.Skip(cursor).Take(headerLength * 2).ToArray();
                    var optimizedParagraph = new OptimizedParagraph(hBytes, localDictionary);
                    optimizedFileData.AddHeader(headerIndex, optimizedParagraph);

                    cursor += hBytes.Length;
                }
            }

            return optimizedFileData;
        }

        public static OptimizedFileData LoadOptimized(string lsFile, CancellationToken ct)
        {
            var lsBytes = File.ReadAllBytes(lsFile);

            return LoadOptimized(lsBytes, ct);
        }
    }
}