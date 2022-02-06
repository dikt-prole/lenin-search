using System.Threading.Tasks;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr.Service
{
    public class FeatureSettingDecorator : IOcrService
    {
        private readonly IOcrService _serviceBase;

        public FeatureSettingDecorator(IOcrService serviceBase)
        {
            _serviceBase = serviceBase;
        }

        public async Task<(OcrPage Page, bool Success, string Error)> GetOcrPageAsync(string imageFile)
        {
            var result = await _serviceBase.GetOcrPageAsync(imageFile);

            if (!result.Success) return result;

            var page = result.Page;

            foreach (var line in page.Lines)
            {
                line.Features = OcrLineFeatures.Calculate(page, line);

                line.Label = line.Features.BelowTopDivider == 0
                    ? OcrLabel.Garbage
                    : line.Features.AboveBottomDivider == 0
                        ? OcrLabel.Comment
                        : OcrLabel.PMiddle;
            }

            return result;
        }
    }
}