using System;
using System.Drawing;
using BookProject.Core.Models.Domain;

namespace BookProject.Core.Misc
{
    public static class BookProjectPalette
    {
        public static Color ImageBlockColor => Color.Orange;
        public static Color CommentLinkBlockColor => Color.DodgerBlue;
        public static Color TitleBlockColor => Color.Red;
        public static Color GarbageBlockColor => Color.Brown;
        public static Color LineFirstBlockColor => Color.Green;
        public static Color LineNormalBlockColor => Color.MediumAquamarine;

        public static Pen GetBlockPen(Block block, int penWidth)
        {
            var blockType = block.GetType();

            if (blockType == typeof(ImageBlock)) return new Pen(ImageBlockColor, penWidth);

            if (blockType == typeof(TitleBlock)) return new Pen(TitleBlockColor, penWidth);

            if (blockType == typeof(GarbageBlock)) return new Pen(GarbageBlockColor, penWidth);

            if (blockType == typeof(CommentLinkBlock)) return new Pen(CommentLinkBlockColor, penWidth);

            if (blockType == typeof(Line))
            {
                var lineBlock = block as Line;

                if (lineBlock.Type == LineType.Normal) return new Pen(LineNormalBlockColor, penWidth);

                if (lineBlock.Type == LineType.First) return new Pen(LineFirstBlockColor, penWidth);
            }

            throw new Exception("Unknown block type");
        }

        public static Brush GetBlockBrush(Block block, int alpha = 255)
        {
            var blockType = block.GetType();

            if (blockType == typeof(ImageBlock))
            {
                var imageColor = Color.FromArgb(alpha, ImageBlockColor);
                return new SolidBrush(imageColor);
            }

            if (blockType == typeof(TitleBlock))
            {
                var titleColor = Color.FromArgb(alpha, TitleBlockColor);
                return new SolidBrush(titleColor);
            }

            if (blockType == typeof(GarbageBlock))
            {
                var garbageColor = Color.FromArgb(alpha, GarbageBlockColor);
                return new SolidBrush(garbageColor);
            }

            if (blockType == typeof(CommentLinkBlock))
            {
                var commentLinkColor = Color.FromArgb(alpha, CommentLinkBlockColor);
                return new SolidBrush(commentLinkColor);
            }

            if (block is Line line)
            {
                if (line.Type == LineType.Normal)
                {
                    var lineNormalColor = Color.FromArgb(alpha, LineNormalBlockColor);
                    return new SolidBrush(lineNormalColor);
                }

                if (line.Type == LineType.First)
                {
                    var lineFirstColor = Color.FromArgb(alpha, LineFirstBlockColor);
                    return new SolidBrush(lineFirstColor);
                }
            }

            throw new Exception("Unknown block type");
        }
    }
}