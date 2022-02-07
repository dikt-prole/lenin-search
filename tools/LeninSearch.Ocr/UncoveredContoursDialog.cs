using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
{
    public partial class UncoveredContoursDialog : Form
    {
        public UncoveredContoursDialog()
        {
            InitializeComponent();

            contours_flp.SizeChanged += (sender, args) =>
            {
                foreach (var ucc in contours_flp.Controls.OfType<UncoveredContourControl>()) ucc.Width = contours_flp.Width - 26;
            };

            ok_btn.Click += (sender, args) =>
            {
                var contourControls = contours_flp.Controls.OfType<UncoveredContourControl>().ToList();
                foreach (var ucc in contourControls)
                {
                    ucc.Contour.Word.Text = ucc.WordText;
                }

                DialogResult = DialogResult.OK;
                Close();
            };
        }

        public void SetContours(List<UncoveredContour> contours)
        {
            contours_flp.Controls.Clear();
            foreach (var contour in contours)
            {
                var contourControl = new UncoveredContourControl {Width = contours_flp.Width - 26, Contour = contour};
                contours_flp.Controls.Add(contourControl);
            }
        }
    }
}
