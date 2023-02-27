using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Domain
{
    public class Line : Block
    {
        [JsonProperty("wds")]
        public List<Word> Words { get; set; }

        [JsonProperty("typ")]
        public LineType Type { get; set; }

        [JsonProperty("rpl")]
        public bool Replace { get; set; }

        [JsonProperty("rpt")]
        public string ReplaceText { get; set; }

        public string GetOriginalTextPreview()
        {
            if (Words == null) return null;

            var wordTexts = Words.OrderBy(w => w.TopLeftX).Select(w => w.Text);

            return string.Join(" ", wordTexts);
        }

        public override string ToString()
        {
            return GetOriginalTextPreview();
        }
    }
}