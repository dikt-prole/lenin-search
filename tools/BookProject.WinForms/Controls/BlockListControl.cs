using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.WinForms.Controls.Detect;
using BookProject.WinForms.Dialogs;
using BookProject.WinForms.Model;

namespace BookProject.WinForms.Controls
{
    public partial class BlockListControl : UserControl
    {
        private readonly Dictionary<int, Action<KeyboardArgs>> KeyboardActions;

        private BindingList<BlockListItem> _blockListItems;

        private BookViewModel _bookVm;
        public BlockListControl()
        {
            InitializeComponent();
            pages_chb.Checked = true;
            pages_chb.CheckedChanged += OnCheckedChanged;

            titles_chb.Checked = true;
            titles_chb.CheckedChanged += OnCheckedChanged;

            comments_chb.Checked = true;
            comments_chb.CheckedChanged += OnCheckedChanged;

            image_chb.Checked = true;
            image_chb.CheckedChanged += OnCheckedChanged;

            line_chb.Checked = true;
            line_chb.CheckedChanged += OnCheckedChanged;

            garbage_chb.Checked = true;
            garbage_chb.CheckedChanged += OnCheckedChanged;

            block_lb.DrawMode = DrawMode.OwnerDrawFixed;
            block_lb.DrawItem += OnDrawItem;

            block_lb.PreviewKeyDown += BlockLbOnPreviewKeyDown;
            block_lb.KeyDown += BlockLbOnKeyDown;
            block_lb.MouseDoubleClick += BlockLbOnMouseDoubleClick;

            KeyboardActions = new Dictionary<int, Action<KeyboardArgs>>
            {
                { 
                    KeyTable.BlockListDown, args =>
                    {
                        if (block_lb.SelectedIndex < block_lb.Items.Count - 1)
                        {
                            block_lb.SelectedIndex += 1;
                        }
                    }
                },
                {
                    KeyTable.BlockListUp, args =>
                    {
                        if (block_lb.SelectedIndex > 0)
                        {
                            block_lb.SelectedIndex -= 1;
                        }
                    }
                },
                {
                    KeyTable.HeadingLevelIncrease, args =>
                    {
                        if (_bookVm.SelectedBlock is TitleBlock titleBlock)
                        {
                            _bookVm.ModifyBlock(this, titleBlock, tb =>
                            {
                                tb.Level += 1;
                            });
                        }
                    }
                },
                {
                    KeyTable.HeadingLevelDecrease, args =>
                    {
                        if (_bookVm.SelectedBlock is TitleBlock titleBlock)
                        {
                            _bookVm.ModifyBlock(this, titleBlock, tb =>
                            {
                                if (tb.Level > 0)
                                {
                                    tb.Level -= 1;
                                }
                            });
                        }
                    }
                },
                {
                    KeyTable.ShowBlockDialog, args =>
                    {
                        if (_bookVm.SelectedBlock is CommentLinkBlock commentLinkBlock)
                        {
                            var dialog = new CommentLinkDialog().Init(commentLinkBlock);
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                _bookVm.ModifyBlock(this, commentLinkBlock, clb => dialog.Apply(clb));
                            }
                        }
                    }
                },
                {
                    KeyTable.SwitchLineType, args =>
                    {
                        if (_bookVm.SelectedBlock is Line line && args.Control)
                        {
                            var targetType = line.Type == LineType.Normal ? LineType.First : LineType.Normal;
                            _bookVm.ModifyBlock(this, line, l => l.Type = targetType);
                        }
                    }
                }
            };
        }

        private void BlockLbOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (_bookVm.SelectedBlock is CommentLinkBlock commentLinkBlock)
            {
                var dialog = new CommentLinkDialog().Init(commentLinkBlock);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _bookVm.ModifyBlock(this, commentLinkBlock, clb => dialog.Apply(clb));
                }
            }
        }

        private void BlockLbOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
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
            RefreshList();
        }

        public void Bind(BookViewModel bookVm)
        {
            if (_bookVm != null)
            {
                _bookVm.BlockAdded -= OnBlockAction;
                _bookVm.BlockRemoved -= BookVmOnBlockRemoved;
                _bookVm.BlockModified -= BookVmOnBlockModified;
                _bookVm.SelectedBlockChanged -= BookVmSelectedBlockChanged;
                _bookVm.KeyboardEvent -= OnBookVmKeyboardEvent;
            }

            _bookVm = bookVm;

            _bookVm.BlockAdded += OnBlockAction;
            _bookVm.BlockRemoved += BookVmOnBlockRemoved;
            _bookVm.BlockModified += BookVmOnBlockModified;
            _bookVm.SelectedBlockChanged += BookVmSelectedBlockChanged;
            _bookVm.KeyboardEvent += OnBookVmKeyboardEvent;

            RefreshList();
        }

        private void BookVmOnBlockRemoved(object sender, Block e)
        {
            var blockListItem = _blockListItems?.FirstOrDefault(b => b.Block == e);
            if (blockListItem != null)
            {
                block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;
                _blockListItems.Remove(blockListItem);
                block_lb.SelectedIndexChanged += OnSelectedIndexChanged;
            }
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

            var blockListItem = _blockListItems.FirstOrDefault(bli => bli.Block == e);
            if (blockListItem != null)
            {
                var index = _blockListItems.IndexOf(blockListItem);
                _blockListItems[index] = new BlockListItem(e);
            }

            block_lb.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        private void BookVmSelectedBlockChanged(object sender, Block e)
        {
            if (sender.GetType() == typeof(DetectLineControl)) return;
            if (sender.GetType() == typeof(DetectCommentLinkControl)) return;
            if (sender.GetType() == typeof(DetectTitleControl)) return;
            if (sender.GetType() == typeof(DetectImageControl)) return;
            if (sender.GetType() == typeof(DetectGarbageControl)) return;

            var blockListItems = block_lb.Items.OfType<BlockListItem>().ToList();
            var selectedBlockListItem = blockListItems.FirstOrDefault(bli => bli.Block == e);
            if (selectedBlockListItem == null)
            {
                RefreshList();
            }
            else
            {
                block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;
                block_lb.SelectedIndex = blockListItems.IndexOf(selectedBlockListItem);
                block_lb.SelectedIndexChanged += OnSelectedIndexChanged;
            }
        }

        private void OnBlockAction(object sender, Block e)
        {
            if (sender.GetType() == typeof(DetectLineControl)) return;
            if (sender.GetType() == typeof(DetectCommentLinkControl)) return;
            if (sender.GetType() == typeof(DetectTitleControl)) return;
            if (sender.GetType() == typeof(DetectImageControl)) return;
            if (sender.GetType() == typeof(DetectGarbageControl)) return;

            if (sender != this)
            {
                RefreshList();
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

        private void RefreshList()
        {
            block_lb.SelectedIndexChanged -= OnSelectedIndexChanged;

            if (_bookVm == null) return;

            _blockListItems = new BindingList<BlockListItem>(GetBlockListItems(_bookVm).ToList());
            var bindingSource = new BindingSource();
            bindingSource.DataSource = _blockListItems;
            block_lb.DataSource = bindingSource;

            var selectedBlockListItem = _blockListItems.FirstOrDefault(b => b.Block == _bookVm.SelectedBlock);
            if (selectedBlockListItem != null)
            {
                block_lb.SelectedIndex = _blockListItems.IndexOf(selectedBlockListItem);
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

        private IEnumerable<BlockListItem> GetBlockListItems(BookViewModel bookVm)
        {
            var pages = bookVm.Book.Pages.OrderBy(p => p.Index).ToArray();
            foreach (var page in pages)
            {
                if (pages_chb.Checked || bookVm.SelectedBlock == page)
                {
                    yield return new BlockListItem(page);
                }

                var blocks = page.GetNonPageBlocks().OrderBy(b => b.TopLeftY).ThenBy(b => b.TopLeftX).ToArray();
                foreach (var block in blocks)
                {
                    var include = block == bookVm.SelectedBlock ||
                                  block is TitleBlock && titles_chb.Checked ||
                                  block is CommentLinkBlock && comments_chb.Checked ||
                                  block is ImageBlock && image_chb.Checked ||
                                  block is Line && line_chb.Checked ||
                                  block is GarbageBlock && garbage_chb.Checked;
                    if (include)
                    {
                        yield return new BlockListItem(block);
                    }
                }
            }
        }
    }
}
