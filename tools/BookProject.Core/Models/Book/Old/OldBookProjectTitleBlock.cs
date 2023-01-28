using System.Collections.Generic;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book.Old
{
    public class OldBookProjectTitleBlock : OldBookProjectBlock
    {
        [JsonProperty("tll")]
        public int TitleLevel { get; set; }

        [JsonProperty("tlt")]
        public string TitleText { get; set; }

        [JsonProperty("ctl")]
        public List<OldBookProjectWord> CommentLinks { get; set; }
    }
}