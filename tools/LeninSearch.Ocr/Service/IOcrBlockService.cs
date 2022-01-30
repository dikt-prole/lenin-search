using System.Collections.Generic;
using System.Threading.Tasks;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public interface IOcrBlockService
    {
        public Task<(List<OcrFeaturedBlock> Blocks, bool Success, string Error)> GetBlocksAsync(string imageFile);
    }
}