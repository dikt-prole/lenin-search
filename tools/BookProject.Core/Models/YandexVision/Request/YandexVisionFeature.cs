using Newtonsoft.Json;

namespace BookProject.Core.Models.YandexVision.Request
{
    public class YandexVisionFeature
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "text_detection_config")]
        public YandexVisionTextDetectionConfig TextDetectionConfig { get; set; }

    }
}