using System.Collections.Generic;

namespace BookProject.Core.Models.YandexVision.Response
{
    public class YandexVisionTextDetection
    {
        public IList<YandexVisionPage> Pages { get; set; }
    }
}