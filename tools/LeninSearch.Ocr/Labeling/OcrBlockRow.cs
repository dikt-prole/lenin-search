using System.Drawing;
using System.IO;
using System.Linq;
using LeninSearch.Ocr.YandexVision.OcrResponse;

namespace LeninSearch.Ocr.Labeling
{
    public class OcrBlockRow
    {
        // block id
        public string FileName { get; set; }
        public int BlockIndex { get; set; }

        // block data
        public int BottomIndent { get; set; }
        public int LeftIndent { get; set; }
        public int RightIndent { get; set; }
        public int TopIndent { get; set; }
        public double PixelsPerSymbol { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double WidthToHeightRatio { get; set; }
        public int WordCount { get; set; }
        public int SymbolCount { get; set; }
        public int SameYLevelBlockCount { get; set; }
        public int BottomLineDistance { get; set; }
        public int TopLineDistance { get; set; }
        public int ImageIndex { get; set; }

        // block label
        public OcrBlockLabel? Label { get; set; }

        public override string ToString()
        {
            if (Label == null) return $"{FileName}-{BlockIndex}";

            return $"{FileName}-{BlockIndex} ({Label})";
        }

        public static OcrBlockRow Construct(Page ocrPage, int blockIndex, string imageFile)
        {
            var block = ocrPage.Blocks[blockIndex];

            var topLeft = block.BoundingBox.TopLeft.Point();
            var topRight = block.BoundingBox.TopRight.Point();
            var bottomLeft = block.BoundingBox.BottomLeft.Point();
            var bottomRight = block.BoundingBox.BottomRight.Point();

            using var image = new Bitmap(Image.FromFile(imageFile));

            var words = block.Lines.SelectMany(l => l.Words).ToList();
            var text = string.Join(" ", words.Select(w => w.Text));
            var totalPixelWidth = block.Lines.Sum(l => l.BoundingBox.TopRight.Point().X - l.BoundingBox.TopLeft.Point().X);
            var pixelsPerSymbol = 1.0 * totalPixelWidth / text.Length;
            var rowWidth = block.BoundingBox.TopRight.Point().X - block.BoundingBox.TopLeft.Point().X;
            var rowHeight = block.BoundingBox.BottomLeft.Point().Y - block.BoundingBox.TopLeft.Point().Y;

            var topDividerLine = CvUtil.GetTopDividerLine(imageFile);
            var bottomDividerLine = CvUtil.GetBottomDividerLine(imageFile);
            var imageIndex = int.Parse(new string(Path.GetFileNameWithoutExtension(imageFile).Where(char.IsNumber).ToArray()));

            var row = new OcrBlockRow
            {
                FileName = Path.GetFileNameWithoutExtension(imageFile),
                BlockIndex = blockIndex,
                PixelsPerSymbol = pixelsPerSymbol,
                LeftIndent = topLeft.X,
                RightIndent = image.Width - topRight.X,
                TopIndent = topLeft.Y,
                BottomIndent = image.Height - bottomLeft.Y,
                SameYLevelBlockCount = ocrPage.Blocks.Count(b => b != block && block.BoundingBox.IsSameY(b.BoundingBox)),
                Width = rowWidth,
                Height = rowHeight,
                WidthToHeightRatio = 1.0 * rowWidth / rowHeight,
                WordCount = words.Count,
                SymbolCount = text.Length,
                TopLineDistance = block.BoundingBox.TopLeft.Point().Y - topDividerLine.Y,
                BottomLineDistance = bottomDividerLine.Y - block.BoundingBox.TopLeft.Point().Y,
                ImageIndex = imageIndex
            };

            return row;
        }
    }
}