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