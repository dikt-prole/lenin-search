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
                using var blockBorderPen = BookProjectPalette.GetBlockBorderPen(block, 4);
                DrawOriginalRect(block.Rectangle, blockBorderPen, g, originalBitmap);
                if (block.State == BlockState.Edit)
                {
                    using var blockElementBrush = BookProjectPalette.GetBlockElementBrush(block);
                    foreach (var dragCenter in block.AllDragCenters())
                    {
                        var dragPbCenter = g.ToPictureBoxPoint(dragCenter, originalBitmap);
                        g.FillRectangle(
                            blockElementBrush, 
                            dragPbCenter.X - Block.PbDragPointSize / 2,
                            dragPbCenter.Y - Block.PbDragPointSize / 2,
                            Block.PbDragPointSize,
                            Block.PbDragPointSize);
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