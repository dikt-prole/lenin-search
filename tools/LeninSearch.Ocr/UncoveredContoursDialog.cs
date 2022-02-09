using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LeninSearch.Ocr.Model;
using LinkLabel = System.Windows.Forms.LinkLabel;

namespace LeninSearch.Ocr
{
    public partial class UncoveredContoursDialog : Form
    {
        private List<UncoveredContour> _contours;
        public UncoveredContoursDialog()
        {
            InitializeComponent();

            pageSize_nud.Minimum = 20;
            pageSize_nud.Maximum = 1000;
            pageSize_nud.Value = 40;
            pageSize_nud.ValueChanged += (sender, args) =>
            {
                if (_contours == null) return;
                SetContours(_contours);
            };

            contours_flp.SizeChanged += (sender, args) =>
            {
                foreach (var ucc in contours_flp.Controls.OfType<UncoveredContourControl>()) ucc.Width = contours_flp.Width - 26;
            };

            ok_btn.Click += (sender, args) =>
            {
                SaveCurrentPage();
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        private void SaveCurrentPage()
        {
            var contourControls = contours_flp.Controls.OfType<UncoveredContourControl>().ToList();
            foreach (var ucc in contourControls)
            {
                ucc.Contour.Word.Text = ucc.WordText;
            }
        }

        public void SetContours(List<UncoveredContour> contours)
        {
            _contours = contours;
            contours_flp.Controls.Clear();
            page_flp.Controls.Clear();
            var pageSize = (int)pageSize_nud.Value;
            var pageCount = contours.Count / pageSize + (contours.Count % pageSize > 0 ? 1 : 0);
            for (var page = 0; page < pageCount; page++)
            {
                var link = new LinkLabel
                {
                    Text = (page + 1).ToString(),
                    LinkColor = Color.Blue,
                    Margin = new Padding(3, 3, 3, 3),
                    Font = new Font(Font.FontFamily, 12, FontStyle.Bold),
                    Width = 25
                };
                link.LinkClicked += LinkOnLinkClicked;
                page_flp.Controls.Add(link);
            }

            LinkOnLinkClicked(page_flp.Controls[0] as LinkLabel, null);
        }

        private void LinkOnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;

            if (link == null) return;

            foreach (var l in page_flp.Controls.OfType<LinkLabel>()) l.LinkColor = Color.Blue;

            link.LinkColor = Color.Red;

            SaveCurrentPage();

            contours_flp.Controls.Clear();

            var page = int.Parse(link.Text) - 1;
            var pageSize = (int)pageSize_nud.Value;
            var pageContours = _contours.Skip(page * pageSize).Take(pageSize).ToList();
            foreach (var contour in pageContours)
            {
                var contourControl = new UncoveredContourControl { Width = contours_flp.Width - 26, Contour = contour };
                contours_flp.Controls.Add(contourControl);
            }
        }
    }
}
