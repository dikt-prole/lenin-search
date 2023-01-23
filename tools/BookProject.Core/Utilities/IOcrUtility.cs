using System.IO;
using System.Threading.Tasks;
using BookProject.Core.Models.Ocr;

namespace BookProject.Core.Utilities
{
    public interface IOcrUtility
    {
        Task<OcrPage> GetPageAsync(byte[] imageBytes);
    }
}