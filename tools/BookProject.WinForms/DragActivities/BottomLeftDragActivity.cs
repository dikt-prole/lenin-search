using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public class BottomLeftDragActivity : IDragActivity
    {
        private readonly Block _block;
        public BottomLeftDragActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.TopLeftX = originalPoint.X;
            _block.BottomRightY = originalPoint.Y;
        }
    }
}