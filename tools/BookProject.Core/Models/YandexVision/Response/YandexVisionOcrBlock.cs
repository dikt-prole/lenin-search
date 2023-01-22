using System.Collections.Generic;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionOcrBlock
    {
        public YandexVisionBoundingBox BoundingBox { get; set; }
        public IList<YandexVisionLine> Lines { get; set; }
    }
}