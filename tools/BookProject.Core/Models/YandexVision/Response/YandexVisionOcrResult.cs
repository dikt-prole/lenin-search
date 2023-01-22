using System.Collections.Generic;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionOcrResult
    {
        public IList<YandexVisionTextDetectionResult> Results { get; set; }
    }
}