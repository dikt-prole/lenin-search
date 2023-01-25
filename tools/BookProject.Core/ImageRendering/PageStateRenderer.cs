using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.Misc;
using BookProject.Core.Models.Book;

namespace BookProject.Core.ImageRendering
{
    public class PageStateRenderer : ImageRendererBase
    {
        private readonly PageState _pageState;
        private readonly Func<Point> _cursorPositionFunc;

        public PageStateRenderer(PageState pageState, Func<Point> cursorPositionFunc)
        {
            _pageState = pageState;
            _cursorPositionFunc = cursorPositionFunc;
        }

        protected override Bitmap RenderOriginalBitmap(string imageFile)
        {
            var originalImage = new Bitmap(Image.FromStream(new MemoryStream(File.ReadAllBytes(imageFile))));
            using var originalGraphics = Graphics.FromImage(originalImage);

            if (_pageState.Page == null) return originalImage;

            var cursorPosition = _cursorPositionFunc();
            if (_pageState.SelectionStartPoint != null)
            {
                var xs = new List<int> { _pageState.SelectionStartPoint.Value.X, cursorPosition.X }.OrderBy(i => i).ToList();
                var ys = new List<int> { _pageState.SelectionStartPoint.Value.Y, cursorPosition.Y }.OrderBy(i => i).ToList();
                var rect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                originalGraphics.DrawRectangle(Pens.Black, rect);
            }

            using var ibPen = new Pen(BookProjectPalette.ImageBlockColor, 2);
            using var ibBrush = new SolidBrush(BookProjectPalette.ImageBlockColor);
            foreach (var ib in _pageState.Page.ImageBlocks)
            {
                originalGraphics.DrawRectangle(ibPen, ib.Rectangle);
                foreach (var dragRectangle in ib.AllDragRectangles())
                {
                    originalGraphics.FillRectangle(ibBrush, dragRectangle);
                }
            }

            using var textBrush = new SolidBrush(Color.DarkViolet);

            using var tbPen = new Pen(BookProjectPalette.TitleBlockColor, 2);
            using var tbBrush = new SolidBrush(BookProjectPalette.TitleBlockColor);
            foreach (var tb in _pageState.Page.TitleBlocks ?? new List<BookProjectTitleBlock>())
            {
                var tbpbRect = tb.Rectangle;
                originalGraphics.DrawRectangle(tbPen, tbpbRect);
                foreach (var dragRectangle in tb.AllDragRectangles())
                {
                    originalGraphics.FillRectangle(tbBrush, dragRectangle);
                }
            }

            using var dividerPen = new Pen(Color.DarkViolet, 2);
            using var dividerBrush = new SolidBrush(Color.DarkViolet);

            var topDividerStart = new Point(_pageState.Page.TopDivider.LeftX, _pageState.Page.TopDivider.Y);
            var topDividerEnd = new Point(_pageState.Page.TopDivider.RightX, _pageState.Page.TopDivider.Y);
            var topDividerDragRectangle = _pageState.Page.TopDivider.DragRectangle;
            originalGraphics.DrawLine(dividerPen, topDividerStart, topDividerEnd);
            originalGraphics.FillRectangle(dividerBrush, topDividerDragRectangle);

            var bottomDividerStart = new Point(_pageState.Page.BottomDivider.LeftX, _pageState.Page.BottomDivider.Y);
            var bottomDividerEnd = new Point(_pageState.Page.BottomDivider.RightX, _pageState.Page.BottomDivider.Y);
            var bottomDividerDragRectangle = _pageState.Page.BottomDivider.DragRectangle;
            originalGraphics.DrawLine(dividerPen, bottomDividerStart, bottomDividerEnd);
            originalGraphics.FillRectangle(dividerBrush, bottomDividerDragRectangle);

            return originalImage;
        }
    }
}