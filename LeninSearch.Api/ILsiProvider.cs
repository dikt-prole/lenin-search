using System.Collections.Generic;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Api
{
    public interface ILsiProvider
    {
        LsIndexData GetLsiData(int corpusVersion, string filePath);

        string[] GetDictionary(int corpusVersion);

        Corpus GetCorpus(int corpusVersion);
    }
}