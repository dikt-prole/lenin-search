using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public class TopRightDragActivity : IDragActivity
    {
        private readonly Block _block;
        public TopRightDragActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.BottomRightX = originalPoint.X;
            _block.TopLeftY = originalPoint.Y;
        }
    }
}