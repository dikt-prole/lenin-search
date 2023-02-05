using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.WinForms.Model;

namespace BookProject.WinForms.Controls
{
    public partial class TitleListControl : UserControl
    {
        public event EventHandler<TitleListRow> TitleSelected; 

        public TitleListControl()
        {
            InitializeComponent();
        }

        public void UpdateTitleList(Book book, TitleBlock selectedBlock)
        {
            title_lb.SelectedIndexChanged -= Title_lbOnSelectedIndexChanged;

            var titleRows = GetTitleRowList(book).ToList();
            if (titleRows.Count != title_lb.Items.Count)
            {
                title_lb.Items.Clear();
                foreach (var titleRow in titleRows)
                {
                    title_lb.Items.Add(titleRow);
                }
            }

            if (selectedBlock != null)
            {
                var selectedRow = titleRows.FirstOrDefault(r => r.TitleBlock == selectedBlock);
                if (selectedRow != null)
                {
                    title_lb.SelectedIndex = titleRows.IndexOf(selectedRow);
                    title_lb.Items[title_lb.SelectedIndex] = selectedRow;
                }
            }

            title_lb.Refresh();

            title_lb.SelectedIndexChanged += Title_lbOnSelectedIndexChanged;
        }

        private void Title_lbOnSelectedIndexChanged(object sender, EventArgs e)
        {
            TitleSelected?.Invoke(this, title_lb.SelectedItem as TitleListRow);
        }

        private IEnumerable<TitleListRow> GetTitleRowList(Book book)
        {
            if (book?.Pages == null) yield break;

            foreach (var page in book.Pages.OrderBy(p => p.Index))
            {
                foreach (var titleBlock in page.TitleBlocks.OrderBy(tb => tb.TopLeftY))
                {
                    yield return new TitleListRow
                    {
                        Page = page,
                        TitleBlock = titleBlock
                    };
                }
            }
        }
    }
}
