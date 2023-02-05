using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.DragActivities
{
    public static class DragActivityFactory
    {
        public static IDragActivity ConstructDragActivity(BookViewModel bookVm, Block block, DragPointLabel dragPointLabel)
        {
            switch (dragPointLabel)
            {
                case DragPointLabel.Left:
                    return new LeftDragActivity(bookVm, block);
                case DragPointLabel.Right:
                    return new RightDragActivity(bookVm, block);
                case DragPointLabel.Bottom:
                    return new BottomDragActivity(bookVm, block);
                case DragPointLabel.Top:
                    return new TopDragActivity(bookVm, block);
                case DragPointLabel.TopLeft:
                    return new TopLeftDragActivity(bookVm, block);
                case DragPointLabel.BottomLeft:
                    return new BottomLeftDragActivity(bookVm, block);
                case DragPointLabel.TopRight:
                    return new TopRightDragActivity(bookVm, block);
                case DragPointLabel.BottomRight:
                    return new BottomRightDragActivity(bookVm, block);
                default:
                    return new CenterDragActivity(bookVm, block);
            }
        }
    }
}