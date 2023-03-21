using System.Collections.Generic;
using System.Linq;
using BookProject.Core.Models.Ocr;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionOcrResponse
    {
        public IList<YandexVisionOcrResult> Results { get; set; }

        public OcrPage ToOcrPage()
        {
            var page = new OcrPage
            {
                Lines = new List<OcrLine>()
            };

            var responseBlocks = new List<YandexVisionOcrBlock>();
            if (Results?.Any() == true)
            {
                if (Results[0].Results?.Any() == true)
                {
                    if (Results[0].Results[0].TextDetection != null)
                    {
                        if (Results[0].Results[0].TextDetection.Pages?.Any() == true)
                        {
                            if (Results[0].Results[0]?.TextDetection?.Pages[0]?.Blocks?.Any() == true)
                            {
                                responseBlocks.AddRange(Results[0].Results[0]?.TextDetection?.Pages[0]?.Blocks);
                            }
                        }
                    }
                }
            }

            if (responseBlocks?.Any() != true)
            {
                return page;
            }

            var pageLines = responseBlocks.SelectMany(b => b.Lines).ToList();
            page.Lines = pageLines.Select(p => p.ToOcrLine()).ToList();

            return page;
        }
    }
}