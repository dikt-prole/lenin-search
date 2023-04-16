using System.Collections.Generic;
using LenLib.Standard.Core.Corpus.Json;
using LenLib.Standard.Core.Corpus.Lsi;

namespace LenLib.Standard.Core.LsiUtil
{
    public interface ILsiUtil
    {
        byte[] ToLsIndexBytes(JsonFileData fd, Dictionary<string, uint> reverseDictionary);
        LsiData FromLsIndexBytes(byte[] lsIndexBytes);
    }
}