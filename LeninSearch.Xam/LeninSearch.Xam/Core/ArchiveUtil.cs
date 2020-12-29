using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeninSearch.Xam.Core.Oprimized;

namespace LeninSearch.Xam.Core
{
    public static class ArchiveUtil
    {
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