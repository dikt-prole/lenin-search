using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Corpus.Lsi;

namespace LenLib.Standard.Core.Search
{
    public interface ILsiProvider
    {
        LsiData GetLsiData(string corpusId, string file);
        LsDictionary GetDictionary(string corpusId);
        CorpusItem GetCorpusItem(string corpusId);
    }
}