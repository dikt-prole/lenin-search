using System.Collections.Generic;
using LeninSearch.Standard.Core.Corpus.Json;
using LeninSearch.Standard.Core.Corpus.Lsi;

namespace LeninSearch.Standard.Core.LsiUtil
{
    public interface ILsiUtil
    {
        byte[] ToLsIndexBytes(JsonFileData fd, Dictionary<string, uint> reverseDictionary);
        LsIndexData FromLsIndexBytes(byte[] lsIndexBytes);
    }
}