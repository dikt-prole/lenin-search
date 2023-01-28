using System;
using System.Drawing;
using BookProject.Core.Models.Book;
using BookProject.Core.Models.Book.Old;
using Emgu.CV.Features2D;

namespace BookProject.Core.Misc
{
    public static class BookProjectPalette
    {
        public static Color GetColor(OldBookProjectLabel? label)
        {
            if (label == null) return Color.Gray;

            switch (label.Value)
            {
                case OldBookProjectLabel.PStart:
                    return Color.Green;

                case OldBookProjectLabel.PMiddle:
                    return Color.MediumAquamarine;

                case OldBookProjectLabel.Garbage:
                    return Color.Brown;

                case OldBookProjectLabel.Image:
                    return Color.Orange;

                case OldBookProjectLabel.Comment:
                    return Color.DodgerBlue;
                
                case OldBookProjectLabel.Title:
                    return Color.Red;
            }

            return Color.Black; 
        }

        public static Color ImageBlockColor => Color.Orange;
        public static Color CommentLinkBlockColor => Color.DodgerBlue;
        public static Color TitleBlockColor => Color.Red;
        public static Color GarbageBlockColor => Color.Brown;
        public static Color LineStartBlockColor => Color.Green;
        public static Color LineNormalBlockColor => Color.MediumAquamarine;

        public static Pen GetBlockBorderPen(Block block, int penWidth)
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

                if (lineBlock.Type == LineType.LineStart) return new Pen(LineStartBlockColor, penWidth);
            }

            throw new Exception("Unknown block type");
        }

        public static Brush GetBlockElementBrush(Block block)
        {
            var blockType = block.GetType();

            if (blockType == typeof(ImageBlock)) return new SolidBrush(ImageBlockColor);

            if (blockType == typeof(TitleBlock)) return new SolidBrush(TitleBlockColor);

            if (blockType == typeof(GarbageBlock)) return new SolidBrush(GarbageBlockColor);

            if (blockType == typeof(CommentLinkBlock)) return new SolidBrush(CommentLinkBlockColor);

            if (blockType == typeof(Line))
            {
                var lineBlock = block as Line;

                if (lineBlock.Type == LineType.Normal) return new SolidBrush(LineNormalBlockColor);

                if (lineBlock.Type == LineType.LineStart) return new SolidBrush(LineStartBlockColor);
            }

            throw new Exception("Unknown block type");
        }
    }
}