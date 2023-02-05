using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.DragActivities
{
    public class CenterDragActivity : IDragActivity
    {
        private readonly BookViewModel _bookVm;
        private readonly Block _block;
        public CenterDragActivity(BookViewModel bookVm, Block block)
        {
            _bookVm = bookVm;
            _block = block;
        }

        public void Perform(PictureBox pictureBox, MouseEventArgs args)
        {
            _bookVm.ModifyBlock(_block, b =>
            {
                var originalPoint = pictureBox.ToOriginalPoint(args.Location);
                var blockWidth = b.BottomRightX - b.TopLeftX;
                var blockHeight = b.BottomRightY - b.TopLeftY;
                b.TopLeftX = originalPoint.X - blockWidth / 2;
                b.TopLeftY = originalPoint.Y - blockHeight / 2;
                b.BottomRightX = originalPoint.X + blockWidth / 2;
                b.BottomRightY = originalPoint.Y + blockHeight / 2;
            });
        }
    }
}