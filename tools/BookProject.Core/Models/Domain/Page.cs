using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookProject.Core.Utilities;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Domain
{
    public class Page
    {
        [JsonProperty("ibl")]
        public List<ImageBlock> ImageBlocks { get; set; }

        [JsonProperty("tbl")]
        public List<TitleBlock> TitleBlocks { get; set; }

        [JsonProperty("clb")]
        public List<CommentLinkBlock> CommentLinkBlocks { get; set; }

        [JsonProperty("gbl")]
        public List<GarbageBlock> GarbageBlocks { get; set; }

        [JsonProperty("lns")]
        public List<Line> Lines { get; set; }

        [JsonProperty("ifl")]
        public string ImageFile { get; set; }

        [JsonProperty("wdt")]
        public int Width { get; set; }

        [JsonProperty("hgt")]
        public int Height { get; set; }

        [JsonIgnore]
        public int Index => int.Parse(new string(ImageFile.Where(char.IsNumber).ToArray()));

        public IEnumerable<Block> GetAllBlocks()
        {
            var blocks = new List<Block>();

            if (ImageBlocks?.Any() == true)
            {
                blocks.AddRange(ImageBlocks);
            }

            if (TitleBlocks?.Any() == true)
            {
                blocks.AddRange(TitleBlocks);
            }

            if (CommentLinkBlocks?.Any() == true)
            {
                blocks.AddRange(CommentLinkBlocks);
            }

            if (GarbageBlocks?.Any() == true)
            {
                blocks.AddRange(GarbageBlocks);
            }

            if (Lines?.Any() == true)
            {
                blocks.AddRange(Lines);
            }

            return blocks;
        }

        public static Page ConstructEmpty(string imageFile)
        {
            using var image = ImageUtility.Load(imageFile);
            return new Page
            {
                ImageFile = Path.GetFileNameWithoutExtension(imageFile),
                ImageBlocks = new List<ImageBlock>(),
                TitleBlocks = new List<TitleBlock>(),
                CommentLinkBlocks = new List<CommentLinkBlock>(),
                GarbageBlocks = new List<GarbageBlock>(),
                Width = image.Width,
                Height = image.Height
            };
        }

        public Block GetEditBlock()
        {
            return GetAllBlocks().FirstOrDefault(b => b.State == BlockState.Edit);
        }
    }
}