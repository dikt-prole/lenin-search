using System;
using System.Threading.Tasks;
using LeninSearch.Ocr.Model;
using Polly;
using Polly.Retry;

namespace LeninSearch.Ocr.Service
{
    public class OcrServiceRetryDecorator : IOcrService
    {
        private readonly IOcrService _serviceBase;

        public OcrServiceRetryDecorator(IOcrService serviceBase)
        {
            _serviceBase = serviceBase;
        }

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            OcrPage page = null;
            bool success = false;
            string error = null;

            await Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1))
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