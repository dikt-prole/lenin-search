using System.Drawing;
using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public static class DragPointLabelResolver
    {
        public static DragPointLabel? GetDragLabelAtPoint(Block block, PictureBox pictureBox, Point pbLocation)
        {
            var activeDragLabels = block.GetActiveDragLabels();
            foreach (var dragLabel in activeDragLabels)
            {
                var pbDragRectangle = block.GetPbDragRectangle(pictureBox.ToPictureBoxPoint(block.GetDragPoint(dragLabel)));
                if (pbDragRectangle.Contains(pbLocation))
                {
                    return dragLabel;
                }
            }

            return null;
        }
    }
}