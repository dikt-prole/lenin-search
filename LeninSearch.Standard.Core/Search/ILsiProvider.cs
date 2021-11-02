using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Standard.Core.Search
{
    public interface ILsiProvider
    {
        LsIndexData GetLsiData(int corpusVersion, string filePath);
        LsDictionary GetDictionary(int corpusVersion);
        Corpus.Corpus GetCorpus(int corpusVersion);
    }
}