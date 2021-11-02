using System.Threading.Tasks;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public interface ICorpusSearch
    {
        Task<PartialParagraphSearchResult> SearchAsync(string corpusName, int corpusVersion, string query, string lastSearchedFilePath);
    }
}