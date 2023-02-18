using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Math;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.WinForms.Model;

namespace BookProject.WinForms.Controls
{
    public partial class BlockListControl : UserControl
    {
        private BookViewModel _bookVm;
        public BlockListControl()
        {
            InitializeComponent();
            pages_chb.Checked = true;
            titles_chb.Checked = true;
            comments_chb.Checked = true;
            block_lb.DrawMode = DrawMode.OwnerDrawFixed;
            block_lb.DrawItem += OnDrawItem;
        }

        public void SetBookVm(BookViewModel bookVm)
        {
            if (_bookVm != null)
            {
                _bookVm.BlockAdded -= OnBlockAction;
                _bookVm.BlockRemoved -= OnBlockAction;
                _bookVm.BlockModified -= OnBlockAction;
                _bookVm.SelectedBlockChanged -= OnBlockAction;
            }

            _bookVm = bookVm;

            _bookVm.BlockAdded += OnBlockAction;
            _bookVm.BlockRemoved += OnBlockAction;
            _bookVm.BlockModified += OnBlockAction;
            _bookVm.SelectedBlockChanged += OnBlockAction;

            RefreshList(null);
        }

        private void OnBlockAction(object sender, Block e)
        {
            RefreshList(_bookVm.SelectedBlock);
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            var blockListItem = block_lb.Items[e.Index] as BlockListItem;
            var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            e.DrawBackground();
            using var backgroundBrush = isSelected 
                ? new SolidBrush(Color.Blue) 
                : new SolidBrush(blockListItem.GetColor());
            using var fontBrush = isSelected
                ? new SolidBrush(Color.White)
                : new SolidBrush(Color.Black);

            e.Graphics.FillRectangle(backgroundBrush, e.Bounds);
            e.Graphics.DrawString(blockListItem.ToString(), e.Font, fontBrush, 0, 0);

            e.DrawFocusRectangle();
        }

        private void RefreshList(Block selectedBlock)
        {
            block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;
            block_lb.Items.Clear();

            var blockListItems = GetBlockListItems(_bookVm.Book).ToArray();
            foreach (var blockListItem in blockListItems)
            {
                block_lb.Items.Add(blockListItem);
            }

            var selectedBlockListItem = blockListItems.FirstOrDefault(b => b.Block == selectedBlock);
            if (selectedBlockListItem != null)
            {
                block_lb.SelectedItem = blockListItems.IndexOf(selectedBlockListItem);
            }

            block_lb.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var blockListItem = block_lb.SelectedItem as BlockListItem;
            _bookVm.SetBlockSelected(blockListItem.Block);
        }

        private IEnumerable<BlockListItem> GetBlockListItems(Book book)
        {
            var pages = book.Pages.OrderBy(p => p.Index).ToArray();
            foreach (var page in pages)
            {
                if (pages_chb.Checked)
                {
                    yield return new BlockListItem(page);
                }

                var blocks = page.GetAllBlocks().OrderBy(b => b.TopLeftY).ThenBy(b => b.TopLeftX).ToArray();
                foreach (var block in blocks)
                {
                    if (block is TitleBlock && titles_chb.Checked)
                    {
                        yield return new BlockListItem(block);
                    }

                    if (block is CommentLinkBlock && comments_chb.Checked)
                    {
                        yield return new BlockListItem(block);
                    }
                }
            }
        }
    }
}
