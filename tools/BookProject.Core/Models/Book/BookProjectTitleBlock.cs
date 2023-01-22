using System.Collections.Generic;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class BookProjectTitleBlock : BookProjectBlock
    {
        [JsonProperty("tll")]
        public int TitleLevel { get; set; }

        [JsonProperty("tlt")]
        public string TitleText { get; set; }

        [JsonProperty("ctl")]
        public List<BookProjectWord> CommentLinks { get; set; }
    }
}