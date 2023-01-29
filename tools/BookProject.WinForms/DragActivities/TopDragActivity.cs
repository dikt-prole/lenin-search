using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public class TopDragActivity : IDragActivity
    {
        private readonly Block _block;
        public TopDragActivity(Block block)
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