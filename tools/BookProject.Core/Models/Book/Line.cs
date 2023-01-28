using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book
{
    public class Line : Block
    {
        [JsonProperty("wds")]
        public List<Word> Words { get; set; }

        [JsonProperty("typ")]
        public LineType Type { get; set; }

        public string GetTextPreview()
        {
            if (Words == null) return null;

            var wordTexts = Words.OrderBy(w => w.TopLeftX).Select(w => w.Text);

            return string.Join(" ", Words.Select(w => w.Text));
        }

        public override string ToString()
        {
            return GetTextPreview();
        }
    }
}