using System.Drawing;
using System.Drawing.Drawing2D;
using BookProject.Core.Misc;
using BookProject.Core.Models.Book;

namespace BookProject.Core.ImageRendering
{
    public class PageStateRenderer : ImageRendererBase
    {
        private readonly PageState _pageState;

        public PageStateRenderer(PageState pageState)
        {
            _pageState = pageState;
        }

        public override void Render(Bitmap originalBitmap, Graphics g)
        {
            if (_pageState.Page == null)
            {
                return;
            }

            var blocks = _pageState.Page.GetAllBlocks();
            foreach (var block in blocks)
            {
                if (block is TitleBlock titleBlock)
                {
                    if (!string.IsNullOrEmpty(titleBlock.Text))
                    {
                        using var titleBlockFillBrush = new SolidBrush(Color.FromArgb(50, BookProjectPalette.TitleBlockColor));
                        FillOriginalRect(block.Rectangle, titleBlockFillBrush, g, originalBitmap);
                    }

                    var pbRectangle = g.ToPictureBoxRectangle(block.Rectangle, originalBitmap);
                    var levelX = pbRectangle.X - 20;
                    var levelY = pbRectangle.Y;
                    using var titleBlockLevelBrush = new SolidBrush(BookProjectPalette.TitleBlockColor);
                    g.DrawString(
                        titleBlock.Level.ToString(), 
                        new Font(FontFamily.GenericSansSerif, 12),
                        titleBlockLevelBrush, 
                        levelX, 
                        levelY );
                }

                if (block is CommentLinkBlock commentLinkBlock)
                {
                    if (!string.IsNullOrEmpty(commentLinkBlock.CommentText))
                    {
                        using var commentLinkBlockFillBrush = new SolidBrush(Color.FromArgb(50, BookProjectPalette.CommentLinkBlockColor));
                        FillOriginalRect(block.Rectangle, commentLinkBlockFillBrush, g, originalBitmap);
                    }
                }

                using var blockBorderPen = BookProjectPalette.GetBlockBorderPen(block, block.BorderWidth);
                DrawOriginalRect(block.Rectangle, blockBorderPen, g, originalBitmap);
                if (block.State == BlockState.Edit)
                {
                    using var blockElementBrush = BookProjectPalette.GetBlockElementBrush(block);
                    foreach (var dragPoint in block.GetActiveDragPoints())
                    {
                        var dragPbPoint = g.ToPictureBoxPoint(dragPoint, originalBitmap);
                        g.FillRectangle(blockElementBrush, block.GetPbDragRectangle(dragPbPoint));
                    }
                }
            }

            if (_pageState.PbSelectionStartPoint.HasValue)
            {
                using var selectionPen = new Pen(Color.Black, 1)
                {
                    DashStyle = DashStyle.Dot
                };
                g.DrawRectangle(selectionPen, _pageState.PbSelectionStartPoint.Value, _pageState.PbMouseAt);
            }
        }
    }
}