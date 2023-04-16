using System.Threading.Tasks;

namespace LenLib.Standard.Core.Search.CorpusSearching
{
    public interface ICorpusSearch
    {
        Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode mode);
    }
}