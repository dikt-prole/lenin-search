using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.MouseMoveActivities
{
    public class DragBlockBottomMouseMoveActivity : IMouseMoveActivity
    {
        private readonly Block _block;
        public DragBlockBottomMouseMoveActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.BottomRightY = originalPoint.Y;
        }
    }
}