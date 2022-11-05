using System.Collections.Generic;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Corpus;

namespace LeninSearch.Xam.Core.Interfaces
{
    public interface IApiService
    {
        Task<(List<CorpusItem> Summary, bool Success, string Error)> GetSummaryAsync(int lsiVersion);
        (List<CorpusItem> Summary, bool Success, string Error) GetSummary(int lsiVersion);
        Task<(byte[] Bytes, bool Success, string Error)> GetFileBytesAsync(string corpusId, string file);
    }
}