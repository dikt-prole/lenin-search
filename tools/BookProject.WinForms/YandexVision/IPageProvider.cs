using System.Threading.Tasks;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.YandexVision
{
    public interface IPageProvider
    {
        public Task<(BookProjectPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile);
    }
}