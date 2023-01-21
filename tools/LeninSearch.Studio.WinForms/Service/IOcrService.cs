using System.Threading.Tasks;
using LeninSearch.Studio.Core.Models;
using LeninSearch.Studio.WinForms.Model;

namespace LeninSearch.Studio.WinForms.Service
{
    public interface IOcrService
    {
        public Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile);
    }
}