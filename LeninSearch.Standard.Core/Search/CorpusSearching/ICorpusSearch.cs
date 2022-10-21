using System.Threading.Tasks;

namespace LeninSearch.Standard.Core.Search.CorpusSearching
{
    public interface ICorpusSearch
    {
        Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode mode);
    }
}