using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.MouseMoveActivities
{
    public class DragBlockTopMouseMoveActivity : IMouseMoveActivity
    {
        private readonly Block _block;
        public DragBlockTopMouseMoveActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.TopLeftY = originalPoint.Y;
        }
    }
}