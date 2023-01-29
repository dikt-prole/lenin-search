using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public class TopLeftDragActivity : IDragActivity
    {
        private readonly Block _block;
        public TopLeftDragActivity(Block block)
        {
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            var originalPoint = pictureBox.ToOriginalPoint(args.Location);
            _block.TopLeftX = originalPoint.X;
            _block.TopLeftY = originalPoint.Y;
        }
    }
}