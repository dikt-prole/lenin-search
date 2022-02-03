using System.Collections.Generic;
using System.Threading.Tasks;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public interface IOcrBlockService
    {
        public Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile);
    }
}