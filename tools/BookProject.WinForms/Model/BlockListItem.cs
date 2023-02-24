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
                return $"{prefix}{titleBlock.Text}";
            }

            if (Block is CommentLinkBlock commentLinkBlock)
            {
                return string.IsNullOrEmpty(commentLinkBlock.CommentText) ? "-" : commentLinkBlock.CommentText;
            }

            return base.ToString();
        }
    }
}