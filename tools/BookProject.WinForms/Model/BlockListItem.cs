using System.Drawing;
using System.Linq;
using BookProject.Core.Misc;
using BookProject.Core.Models.Domain;

namespace BookProject.WinForms.Model
{
    public class BlockListItem
    {
        public BlockListItem(Block block)
        {
            Block = block;
        }

        public Block Block { get; set; }

        public Color GetColor()
        {
            if (Block is TitleBlock)
            {
                return Color.FromArgb(50, BookProjectPalette.TitleBlockColor);
            }

            if (Block is CommentLinkBlock)
            {
                return Color.FromArgb(50, BookProjectPalette.CommentLinkBlockColor);
            }

            if (Block is ImageBlock)
            {
                return Color.FromArgb(50, BookProjectPalette.ImageBlockColor);
            }

            if (Block is Line line)
            {
                return Color.FromArgb(50, line.Type == LineType.Normal 
                    ? BookProjectPalette.LineNormalBlockColor 
                    : BookProjectPalette.LineStartBlockColor);
            }

            if (Block is GarbageBlock)
            {
                return Color.FromArgb(50, BookProjectPalette.GarbageBlockColor);
            }

            return Color.White;
        }

        public override string ToString()
        {
            if (Block is Page page)
            {
                return page.ImageFile;
            }

            if (Block is TitleBlock titleBlock)
            {
                var prefix = string.Join("", Enumerable.Repeat("    ", titleBlock.Level));
                var titleText = string.IsNullOrEmpty(titleBlock.Text) ? "-" : titleBlock.Text;
                return $"{prefix}{titleText}";
            }

            if (Block is CommentLinkBlock commentLinkBlock)
            {
                return string.IsNullOrEmpty(commentLinkBlock.CommentText) ? "-" : commentLinkBlock.CommentText;
            }

            if (Block is ImageBlock imageBlock)
            {
                return $"{imageBlock.Rectangle.Width}x{imageBlock.Rectangle.Height}";
            }

            if (Block is Line line)
            {
                var textPreview = line.GetOriginalTextPreview();
                return string.IsNullOrEmpty(textPreview) ? "-" : textPreview;
            }

            if (Block is GarbageBlock garbageBlock)
            {
                return $"{garbageBlock.Rectangle.Width}x{garbageBlock.Rectangle.Height}";
            }

            return base.ToString();
        }
    }
}