using System.Windows.Forms;

namespace BookProject.WinForms.DragActivities
{
    public interface IDragActivity
    {
        void Perform(PictureBox pictureBox, MouseEventArgs args);
    }
}