using System.Collections.Generic;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;

namespace LeninSearch.Standard.Core.Api
{
    public interface IApiClientV1
    {
        Task<(List<CorpusItem> Summary, bool Success, string Error)> GetSummaryAsync(int lsiVersion);
        (List<CorpusItem> Summary, bool Success, string Error) GetSummary(int lsiVersion);
        Task<(byte[] Bytes, bool Success, string Error)> GetFileBytesAsync(string corpusId, string file);
        Task<SearchResult> SearchAsync(string corpusId, string query, SearchMode searchMode);
    }
}