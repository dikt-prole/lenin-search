using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public class BottomRightDragActivity : IDragActivity
    {
        private readonly Block _block;
        public BottomRightDragActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.BottomRightX = originalPoint.X;
            _block.BottomRightY = originalPoint.Y;
        }
    }
}