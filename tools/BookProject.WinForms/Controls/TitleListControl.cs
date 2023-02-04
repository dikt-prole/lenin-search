using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Models.Book;
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

            title_lb.Items.Clear();
            var titleRows = GetTitleRowList(book).ToList();
            foreach (var titleRow in titleRows)
            {
                title_lb.Items.Add(titleRow);
            }

            if (selectedBlock != null)
            {
                var selectedRow = titleRows.FirstOrDefault(r => r.TitleBlock == selectedBlock);
                if (selectedRow != null)
                {
                    title_lb.SelectedIndex = titleRows.IndexOf(selectedRow);
                }
            }

            title_lb.SelectedIndexChanged += Title_lbOnSelectedIndexChanged;
        }

        private void Title_lbOnSelectedIndexChanged(object sender, EventArgs e)
        {
            TitleSelected?.Invoke(this, title_lb.SelectedItem as TitleListRow);
        }

        private IEnumerable<TitleListRow> GetTitleRowList(Book book)
        {
            if (book?.Pages == null) yield break;

            foreach (var page in book.Pages)
            {
                foreach (var titleBlock in page.TitleBlocks)
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
