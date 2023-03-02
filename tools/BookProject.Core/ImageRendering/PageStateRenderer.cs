using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using BookProject.Core.Misc;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.Core.ImageRendering
{
    public class PageStateRenderer : ImageRendererBase
    {
        private readonly BookViewModel _bookVm;

        public PageStateRenderer(BookViewModel bookVm)
        {
            _bookVm = bookVm;
        }

        public override void Render(Bitmap originalBitmap, Graphics g)
        {
            if (_bookVm.CurrentPage == null)
            {
                return;
            }

            var blocks = _bookVm.CurrentPage.GetAllBlocks().Where(b => !(b is Page)).ToArray();
            foreach (var block in blocks)
            {
                /*
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
                */

                /*
                if (block is CommentLinkBlock commentLinkBlock)
                {
                    if (!string.IsNullOrEmpty(commentLinkBlock.CommentText))
                    {
                        using var commentLinkBlockFillBrush = new SolidBrush(Color.FromArgb(50, BookProjectPalette.CommentLinkBlockColor));
                        FillOriginalRect(block.Rectangle, commentLinkBlockFillBrush, g, originalBitmap);
                    }
                }
                */

                using var blockBrush = BookProjectPalette.GetBlockBrush(block, 80);
                FillOriginalRect(block.Rectangle, blockBrush, g, originalBitmap);
                if (block == _bookVm.SelectedBlock)
                {
                    using var selectedPen = BookProjectPalette.GetBlockPen(block, 1);
                    DrawOriginalRect(block.Rectangle, selectedPen, g, originalBitmap);

                    using var selectedBrush = BookProjectPalette.GetBlockBrush(block, 255);
                    foreach (var dragPoint in block.GetActiveDragPoints())
                    {
                        var dragPbPoint = g.ToPictureBoxPoint(dragPoint, originalBitmap);
                        g.FillRectangle(selectedBrush, block.GetPbDragRectangle(dragPbPoint));
                    }
                }
            }

            if (_bookVm.PbSelectionStartPoint.HasValue)
            {
                using var selectionPen = new Pen(Color.Black, 1)
                {
                    DashStyle = DashStyle.Dot
                };
                g.DrawRectangle(selectionPen, _bookVm.PbSelectionStartPoint.Value, _bookVm.PictureBoxMouseAt);
            }
        }
    }
}