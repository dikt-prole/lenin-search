using System;
using System.Threading.Tasks;
using BookProject.Core.Models.Book;
using BookProject.WinForms.YandexVision;
using Polly;

namespace BookProject.WinForms.Service
{
    public class PageProviderRetryDecorator : IPageProvider
    {
        private readonly IPageProvider _serviceBase;

        public PageProviderRetryDecorator(IPageProvider serviceBase)
        {
            _serviceBase = serviceBase;
        }

        public async Task<(BookProjectPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            BookProjectPage page = null;
            bool success = false;
            string error = null;

            var backoff = new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15)
            };

            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(backoff)
                .ExecuteAsync(async () =>
                {
                    var result = await _serviceBase.GetOcrPageAsync(imageFile);
                    page = result.Page;
                    success = result.Success;
                    error = result.Error;
                });

            return (page, success, error);
        }
    }
}