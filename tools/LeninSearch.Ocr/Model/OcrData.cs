using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace LeninSearch.Ocr.Model
{
    public class OcrData
    {
        public List<OcrFeaturedBlock> FeaturedBlocks { get; set; }
        public List<OcrImageBlock> ImageBlocks { get; set; }
        public List<OcrCommentLink> CommentLinks { get; set; }

        public static OcrData Empty()
        {
            return new OcrData
            {
                FeaturedBlocks = new List<OcrFeaturedBlock>(),
                ImageBlocks = new List<OcrImageBlock>(),
                CommentLinks = new List<OcrCommentLink>()
            };
        }

        public static OcrData Load(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "ocr-data.json");
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<OcrData>(json);
            }

            return Empty();
        }

        public void Save(string bookFolder)
        {
            var file = Path.Combine(bookFolder, "ocr-data.json");
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(file, json);
        }
    }
}