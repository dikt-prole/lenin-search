using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LeninSearch.Studio.WinForms
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

    public class YtVisionRequest
    {
        [JsonProperty(PropertyName = "folderId")]
        public string FolderId { get; set; }

        [JsonProperty(PropertyName = "analyze_specs")]
        public List<YtVisionSpec> AnalyzeSpecs { get; set; }

        public static YtVisionRequest Ocr(byte[] imageBytes)
        {
            var content = Convert.ToBase64String(imageBytes);

            var request = new YtVisionRequest
            {
                FolderId = "b1gohu36ujjsr54v1ho5",
                AnalyzeSpecs = new List<YtVisionSpec>
                {
                    new YtVisionSpec
                    {
                        Content = content,
                        Features = new List<YtVisionFeature>
                        {
                            new YtVisionFeature
                            {
                                Type = "TEXT_DETECTION",
                                TextDetectionConfig = new YtVisionTextDetectionConfig
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

    public class YtVisionSpec
    {
        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<YtVisionFeature> Features { get; set; }
    }

    public class YtVisionFeature
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "text_detection_config")]
        public YtVisionTextDetectionConfig TextDetectionConfig { get; set; }

    }

    public class YtVisionTextDetectionConfig
    {
        [JsonProperty(PropertyName = "language_codes")]
        public List<string> LanguageCodes { get; set; }
    }
}