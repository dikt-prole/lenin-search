using System.Drawing;
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
                DrawRect(block.Rectangle, blockBorderPen, g, originalBitmap);
                if (block.State == BlockState.Edit)
                {
                    using var blockElementBrush = BookProjectPalette.GetBlockElementBrush(block);
                    foreach (var dragRectangle in block.AllDragRectangles())
                    {
                        FillRect(dragRectangle, blockElementBrush, g, originalBitmap);   
                    }
                }
            }
        }
    }
}