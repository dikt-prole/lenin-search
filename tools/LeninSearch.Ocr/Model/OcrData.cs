using System.Collections.Generic;

namespace LeninSearch.Ocr.Model
{
    public class OcrData
    {
        public List<OcrFeaturedBlock> FeaturedBlocks { get; set; }
        public List<OcrImageBlock> ImageBlocks { get; set; }

        public static OcrData Empty()
        {
            return new OcrData
            {
                FeaturedBlocks = new List<OcrFeaturedBlock>(),
                ImageBlocks = new List<OcrImageBlock>()
            };
        }
    }
}