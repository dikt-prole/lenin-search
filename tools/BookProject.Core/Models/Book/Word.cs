using System.Drawing;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class Word : Block
    {
        [JsonProperty("txt")]
        public string Text { get; set; }
    }
}