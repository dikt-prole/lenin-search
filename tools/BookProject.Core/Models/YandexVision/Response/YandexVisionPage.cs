using System.Collections.Generic;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionPage
    {
        public IList<YandexVisionOcrBlock> Blocks { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }
}