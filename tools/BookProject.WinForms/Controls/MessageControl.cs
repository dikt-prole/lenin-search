using BookProject.Core.Models;
using BookProject.Core.Models.ViewModel;
using System.Drawing;
using System.Windows.Forms;

namespace BookProject.WinForms.Controls
{
    public partial class MessageControl : UserControl
    {
        private BookViewModel _bookVm;

        public MessageControl()
        {
            InitializeComponent();
            toolTip1.OwnerDraw = true;
            toolTip1.Popup += ToolTip1OnPopup;
            toolTip1.Draw += ToolTip1OnDraw;
        }

        private void ToolTip1OnDraw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            e.DrawBorder();
            e.DrawText();
        }

        private void ToolTip1OnPopup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = new Size(panel1.Width, 40);
        }

        public void Bind(BookViewModel bookVm)
        {
            if (_bookVm != null)
            {
                _bookVm.Message -= BookVmOnMessage;
            }

            _bookVm = bookVm;

            _bookVm.Message += BookVmOnMessage;
        }

        private void BookVmOnMessage(object sender, MessageArgs e)
        {
            toolTip1.BackColor = e.MessageType == MessageType.Error
                ? Color.PaleVioletRed
                : Color.LightGreen;
            toolTip1.Show(e.Text, panel1, 0, 0, 2000);
        }
    }
}
