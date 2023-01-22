using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BookProject.Core.Models.YandexVision.Request
{
    // https://cloud.yandex.ru/docs/vision/quickstart
    /*
     {
        "folderId": "b1gvmob95yysaplct532",
        "analyze_specs": [{
            "content": "iVBORw0KGgo...",
            "features": [{
                "type": "TEXT_DETECTION",
                "text_detection_config": {
                    "language_codes": ["*"]
                }
            }]
        }]
    }   
     */

    public class YandexVisionRequest
    {
        [JsonProperty(PropertyName = "folderId")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "analyze_specs")]
        public List<YandexVisionSpec> AnalyzeSpecs { get; set; }

        public static YandexVisionRequest FromImageBytes(byte[] imageBytes)
        {
            var content = Convert.ToBase64String(imageBytes);

            var request = new YandexVisionRequest
            {
                FolderId = "b1gohu36ujjsr54v1ho5",
                AnalyzeSpecs = new List<YandexVisionSpec>
                {
                    new YandexVisionSpec
                    {
                        Content = content,
                        Features = new List<YandexVisionFeature>
                        {
                            new YandexVisionFeature
                            {
                                Type = "TEXT_DETECTION",
                                TextDetectionConfig = new YandexVisionTextDetectionConfig
                                {
                                    LanguageCodes = new List<string> {"en", "ru"}
                                }
                            }
                        }
                    }
                }
            };

            return request;
        }
    }
}