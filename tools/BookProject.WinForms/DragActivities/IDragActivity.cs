using System.Windows.Forms;

namespace BookProject.WinForms.DragActivities
{
    public interface IDragActivity
    {
        void Perform(object sender, PictureBox pictureBox, MouseEventArgs args);
    }
}