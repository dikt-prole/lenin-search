using System.Collections.Generic;
using Newtonsoft.Json;

namespace BookProject.Core.Models.YandexVision.Request
{
    public class YandexVisionSpec
    {
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<YandexVisionFeature> Features { get; set; }
    }
}