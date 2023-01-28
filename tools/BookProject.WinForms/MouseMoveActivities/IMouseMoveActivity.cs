using System.Windows.Forms;

namespace BookProject.WinForms.MouseMoveActivities
{
    public interface IMouseMoveActivity
    {
        void Perform(PictureBox pictureBox, MouseEventArgs args);
    }
}