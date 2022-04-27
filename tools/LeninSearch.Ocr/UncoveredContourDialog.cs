using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LeninSearch.Ocr.Model;

namespace LeninSearch.Ocr
{
    public partial class UncoveredContourDialog : Form
    {
        private List<UncoveredContour> _contours;

        private int PageCount => _contours.Count / (int)pageSize_nud.Value + (_contours.Count % (int)pageSize_nud.Value > 0 ? 1 : 0);

        public UncoveredContourDialog()
        {
            InitializeComponent();

            pageSize_nud.Minimum = 20;
            pageSize_nud.Maximum = 1000;
            pageSize_nud.Value = 27;
            pageSize_nud.ValueChanged += (sender, args) =>
            {
                if (_contours == null) return;
                SetContours(_contours);
            };

            page_nud.Minimum = 1;
            page_nud.Maximum = 999;
            page_nud.Value = 1;
            page_nud.ValueChanged += PageValueChanged;

            next_btn.Click += (sender, args) =>
            {
                if (page_nud.Value == page_nud.Maximum) return;
                page_nud.Value += 1;
            };

            prev_btn.Click += (sender, args) =>
            {
                if (page_nud.Value == page_nud.Minimum) return;
                page_nud.Value -= 1;
            };

            contours_flp.SizeChanged += (sender, args) =>
            {
                foreach (var ucc in contours_flp.Controls.OfType<CommentLinkControl>()) ucc.Width = contours_flp.Width - 26;
            };

            ok_btn.Click += (sender, args) =>
            {
                SaveCurrentPage();
                DialogResult = DialogResult.OK;
                Close();
            };
        }

        private void PageValueChanged(object? sender, EventArgs e)
        {
            SaveCurrentPage();

            contours_flp.Controls.Clear();

            var page = (int) page_nud.Value - 1;
            var pageSize = (int)pageSize_nud.Value;
            var pageContours = _contours.Skip(page * pageSize).Take(pageSize).ToList();
            foreach (var contour in pageContours)
            {
                var contourControl = new CommentLinkControl { Width = contours_flp.Width - 26, Contour = contour };
                contours_flp.Controls.Add(contourControl);
            }
        }

        private void SaveCurrentPage()
        {
            var contourControls = contours_flp.Controls.OfType<CommentLinkControl>().ToList();
            foreach (var ucc in contourControls)
            {
                ucc.Contour.Word.Text = ucc.WordText;
            }
        }

        public void SetContours(List<UncoveredContour> contours)
        {
            _contours = contours;
            contours_flp.Controls.Clear();

            if (!_contours.Any()) return;

            page_nud.Maximum = PageCount;
            page_nud.Value = 1;
            totalPages_lbl.Text = $"of {PageCount}";
            PageValueChanged(null, null);
        }
    }
}
