using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Math;
using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.WinForms.Model;

namespace BookProject.WinForms.Controls
{
    public partial class BlockListControl : UserControl
    {
        private readonly Dictionary<int, Action<KeyboardArgs>> KeyboardActions;

        private BookViewModel _bookVm;
        public BlockListControl()
        {
            InitializeComponent();
            pages_chb.Checked = true;
            titles_chb.Checked = true;
            comments_chb.Checked = true;
            block_lb.DrawMode = DrawMode.OwnerDrawFixed;
            block_lb.DrawItem += OnDrawItem;
            pages_chb.CheckedChanged += OnCheckedChanged;
            titles_chb.CheckedChanged += OnCheckedChanged;
            comments_chb.CheckedChanged += OnCheckedChanged;
            block_lb.KeyDown += BlockLbOnKeyDown;

            KeyboardActions = new Dictionary<int, Action<KeyboardArgs>>
            {
                { KeyTable.BlockListDown, args =>
                {
                    if (block_lb.SelectedIndex < block_lb.Items.Count - 1)
                    {
                        block_lb.SelectedIndex += 1;
                    }
                }
                },
                { KeyTable.BlockListUp, args =>
                {
                    if (block_lb.SelectedIndex > 0)
                    {
                        block_lb.SelectedIndex -= 1;
                    }
                }
                }
            };
        }

        private void BlockLbOnKeyDown(object sender, KeyEventArgs args)
        {
            if (_bookVm == null) return;

            args.SuppressKeyPress = true;
            args.Handled = true;

            if (KeyboardActions.ContainsKey(args.KeyValue))
            {
                KeyboardActions[args.KeyValue].Invoke(args.ToKeyboardArgs());
            }
            else
            {
                _bookVm.RegisterKeyboardEvent(this, args.ToKeyboardArgs());
            }
        }

        private void OnCheckedChanged(object sender, EventArgs e)
        {
            RefreshList(null);
        }

        public void Bind(BookViewModel bookVm)
        {
            if (_bookVm != null)
            {
                _bookVm.BlockAdded -= OnBlockAction;
                _bookVm.BlockRemoved -= OnBlockAction;
                _bookVm.BlockModified -= OnBlockAction;
                _bookVm.SelectedBlockChanged -= OnBlockAction;
                _bookVm.KeyboardEvent -= OnBookVmKeyboardEvent;
            }

            _bookVm = bookVm;

            _bookVm.BlockAdded += OnBlockAction;
            _bookVm.BlockRemoved += OnBlockAction;
            _bookVm.BlockModified += BookVmOnBlockModified;
            _bookVm.SelectedBlockChanged += BookVmSelectedBlockChanged;
            _bookVm.KeyboardEvent += OnBookVmKeyboardEvent;

            RefreshList(null);
        }

        private void OnBookVmKeyboardEvent(object sender, KeyboardArgs args)
        {
            if (KeyboardActions.ContainsKey(args.KeyValue))
            {
                KeyboardActions[args.KeyValue].Invoke(args);
            }
        }

        private void BookVmOnBlockModified(object sender, Block e)
        {
            block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;

            var blockListItems = block_lb.Items.OfType<BlockListItem>().ToArray();
            var selectedBlockListItem = blockListItems.FirstOrDefault(bli => bli.Block == e);
            if (selectedBlockListItem != null)
            {
                var index = blockListItems.IndexOf(selectedBlockListItem);
                block_lb.Items[index] = selectedBlockListItem;
            }

            block_lb.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void BookVmSelectedBlockChanged(object sender, Block e)
        {
            block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;

            var blockListItems = block_lb.Items.OfType<BlockListItem>().ToArray();
            var selectedBlockListItem = blockListItems.FirstOrDefault(bli => bli.Block == e);
            block_lb.SelectedIndex = selectedBlockListItem == null
                ? -1
                : blockListItems.IndexOf(selectedBlockListItem);

            block_lb.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void OnBlockAction(object sender, Block e)
        {
            if (sender != this)
            {
                RefreshList(_bookVm.SelectedBlock);
            }
        }

        private void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index >= 0 && e.Index < block_lb.Items.Count)
            {
                var itemRectangle = block_lb.GetItemRectangle(e.Index);


                var blockListItem = block_lb.Items[e.Index] as BlockListItem;
                var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                using var backgroundBrush = isSelected
                    ? new SolidBrush(Color.FromArgb(255, 0, 95, 184))
                    : new SolidBrush(blockListItem.GetColor());
                using var fontBrush = isSelected
                    ? new SolidBrush(Color.White)
                    : new SolidBrush(Color.Black);

                e.Graphics.FillRectangle(backgroundBrush, itemRectangle);
                e.Graphics.DrawString(blockListItem.ToString(), e.Font, fontBrush, itemRectangle.Location);
            }

            e.DrawFocusRectangle();
        }

        private void RefreshList(Block selectedBlock)
        {
            block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;
            block_lb.Items.Clear();

            if (_bookVm == null) return;

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
            if (blockListItem != null)
            {
                _bookVm.SetBlockSelected(this, blockListItem.Block);
            }
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
