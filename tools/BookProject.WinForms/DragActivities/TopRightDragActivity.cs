using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.DragActivities
{
    public class TopRightDragActivity : IDragActivity
    {
        private readonly BookViewModel _bookVm;
        private readonly Block _block;
        public TopRightDragActivity(BookViewModel bookVm, Block block)
        {
            _bookVm = bookVm;
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            _bookVm.ModifyBlock(_block, b =>
            {
                var originalPoint = pictureBox.ToOriginalPoint(args.Location);
                b.BottomRightX = originalPoint.X;
                b.TopLeftY = originalPoint.Y;
            });
        }
    }
}