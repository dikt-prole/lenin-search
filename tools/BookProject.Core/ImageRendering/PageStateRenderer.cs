using System;
using System.Drawing;
using BookProject.Core.Misc;
using BookProject.Core.Models.Book;
using BookProject.Core.Utilities;

namespace BookProject.Core.ImageRendering
{
    public class PageStateRenderer : ImageRendererBase
    {
        private readonly PageState _pageState;

        public PageStateRenderer(PageState pageState)
        {
            _pageState = pageState;
        }

        protected override Bitmap RenderOriginalBitmap(string imageFile)
        {
            var originalBitmap = ImageUtility.Load(imageFile);
            if (_pageState.Page == null)
            {
                return originalBitmap;
            }

            using var g = Graphics.FromImage(originalBitmap);
            var blocks = _pageState.Page.GetAllBlocks();
            foreach (var block in blocks)
            {
                using var blockBorderPen = BookProjectPalette.GetBlockBorderPen(block, 4);
                g.DrawRectangle(blockBorderPen, block.Rectangle);
                if (block.State == BlockState.Edit)
                {
                    using var blockElementBrush = BookProjectPalette.GetBlockElementBrush(block);
                    foreach (var dragRectangle in block.AllDragRectangles())
                    {
                        g.FillRectangle(blockElementBrush, dragRectangle);   
                    }
                }
            }

            return originalBitmap;
        }
    }
}