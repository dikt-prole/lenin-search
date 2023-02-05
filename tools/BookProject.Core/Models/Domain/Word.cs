using Newtonsoft.Json;

namespace BookProject.Core.Models.Domain
{
    public class Word : Block
    {
        [JsonProperty("txt")]
        public string Text { get; set; }
    }
}