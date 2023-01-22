using System.Collections.Generic;
using Newtonsoft.Json;

namespace BookProject.Core.Models.YandexVision.Request
{
    public class YandexVisionTextDetectionConfig
    {
        [JsonProperty(PropertyName = "language_codes")]
        public List<string> LanguageCodes { get; set; }
    }
}