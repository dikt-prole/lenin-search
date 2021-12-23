using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Optimized;

namespace LeninSearch.Standard.Core.Search
{
    public interface ILsiProvider
    {
        LsIndexData GetLsiData(string corpusId, string file);
        LsDictionary GetDictionary(string corpusId);
        CorpusItem GetCorpusItem(string corpusId);
    }
}