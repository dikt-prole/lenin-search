using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public class CenterDragActivity : IDragActivity
    {
        private readonly Block _block;
        public CenterDragActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            var blockWidth = _block.BottomRightX - _block.TopLeftX;
            var blockHeight = _block.BottomRightY - _block.TopLeftY;
            _block.TopLeftX = originalPoint.X - blockWidth / 2;
            _block.TopLeftY = originalPoint.Y - blockHeight / 2;
            _block.BottomRightX = originalPoint.X + blockWidth / 2;
            _block.BottomRightY = originalPoint.Y + blockHeight / 2;
        }
    }
}