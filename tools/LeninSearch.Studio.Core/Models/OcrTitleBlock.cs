using System.Collections.Generic;
using Newtonsoft.Json;

namespace LeninSearch.Studio.Core.Models
{
    public class OcrTitleBlock : OcrBlock
    {
        [JsonProperty("tll")]
        public int TitleLevel { get; set; }

        [JsonProperty("tlt")]
        public string TitleText { get; set; }

        [JsonProperty("ctl")]
        public List<OcrWord> CommentLinks { get; set; }
    }
}