using System;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
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