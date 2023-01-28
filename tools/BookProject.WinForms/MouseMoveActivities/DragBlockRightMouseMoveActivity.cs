using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.MouseMoveActivities
{
    public class DragBlockRightMouseMoveActivity : IMouseMoveActivity
    {
        private readonly Block _block;

        public DragBlockRightMouseMoveActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.BottomRightX = originalPoint.X;
        }
    }
}