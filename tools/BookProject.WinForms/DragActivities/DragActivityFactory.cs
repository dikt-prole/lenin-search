using System.Drawing;
using System.Windows.Forms;
using BookProject.Core.Models.Book;

namespace BookProject.WinForms.DragActivities
{
    public static class DragActivityFactory
    {
        public static IDragActivity ConstructDragActivity(Block block, DragPointLabel dragPointLabel)
        {
            switch (dragPointLabel)
            {
                case DragPointLabel.Left:
                    return new LeftDragActivity(block);
                case DragPointLabel.Right:
                    return new RightDragActivity(block);
                case DragPointLabel.Bottom:
                    return new BottomDragActivity(block);
                case DragPointLabel.Top:
                    return new TopDragActivity(block);
                case DragPointLabel.TopLeft:
                    return new TopLeftDragActivity(block);
                case DragPointLabel.BottomLeft:
                    return new BottomLeftDragActivity(block);
                case DragPointLabel.TopRight:
                    return new TopRightDragActivity(block);
                case DragPointLabel.BottomRight:
                    return new BottomRightDragActivity(block);
                default:
                    return new CenterDragActivity(block);
            }
        }
    }
}