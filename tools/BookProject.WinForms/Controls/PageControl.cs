using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.ImageRendering;
using BookProject.Core.Misc;
using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Utilities;
using BookProject.WinForms.Dialogs;
using BookProject.WinForms.DragActivities;
using BookProject.WinForms.KeyboardActions;
using BookProject.WinForms.KeyboardActions.Clipboard;
using BookProject.WinForms.KeyboardActions.MoveResize;

namespace BookProject.WinForms.Controls
{
    public partial class PageControl : UserControl
    {
        private readonly Dictionary<int, Action<KeyboardArgs>> KeyboardActions;
        private IDragActivity _dragActivity;
        private BookViewModel _bookVm;
        private readonly ToolTip _commentToolTip;
        private const int CommentToolTipDuration = 15000;
        public const int CommentToolTipBorderWidth = 3;
        public PageControl()
        {
            InitializeComponent();
            page_pb.Paint += PagePbOnPaint;
            page_pb.MouseDown += PagePbOnMouseDown;
            page_pb.MouseUp += PagePbOnMouseUp;
            page_pb.MouseMove += PagePbOnMouseMove;
            page_pb.KeyDown += PagePbOnKeyDown;
            page_pb.PreviewKeyDown += PagePbOnPreviewKeyDown;
            KeyboardActions = new Dictionary<int, Action<KeyboardArgs>>
            {
                { KeyTable.NextBlock, args => new SetNextBlockSelected().Execute(this, _bookVm, args) },
                { KeyTable.DeleteBlock, args => new RemoveSelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.MoveBlockUp, args => new MoveupOrDownsizeSelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.MoveBlockDown, args => new MovedownOrUpsizeSelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.MoveBlockLeft, args => new MoveleftOrDownsizeSelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.MoveBlockRight, args => new MoverightOrUpsizeSelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.AddImageBlock, args => new AddBlock<ImageBlock>(200, 200).Execute(this, _bookVm, args) },
                { KeyTable.AddTitleBlock, args => new AddBlock<TitleBlock>(100, 100).Execute(this, _bookVm, args) },
                { KeyTable.AddGarbageBlock, args => new AddBlock<GarbageBlock>(100, 100).Execute(this, _bookVm, args) },
                { KeyTable.AddCommentLinkBlock, args => new AddBlock<CommentLinkBlock>(25, 25).Execute(this, _bookVm, args) },
                { KeyTable.AddLineBlock, args => new AddBlock<Line>(200, 25).Execute(this, _bookVm, args) },
                { KeyTable.CopyBlock, args => new CopySelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.PasteBlock, args => new PasteSelectedBlock().Execute(this, _bookVm, args) },
                { KeyTable.DoOcr, args => new BlockOcrAction().Execute(this, _bookVm, args) },
                { KeyTable.SwitchLineType, args => new SwitchLineTypeAction().Execute(this, _bookVm, args) },
                { KeyTable.TitleLevelIncrease, args => new TitleLevelIncreaseAction().Execute(this, _bookVm, args) },
                { KeyTable.TitleLevelDecrease, args => new TitleLevelDecreaseAction().Execute(this, _bookVm, args) }
            };

            _commentToolTip = new ToolTip();
            _commentToolTip.OwnerDraw = true;
            _commentToolTip.Popup += CommentToolTipOnPopup;
            _commentToolTip.Draw += CommentToolTipOnDraw;
            _commentToolTip.BackColor = BookProjectPalette.CommentLinkBlockColor;
        }

        private void CommentToolTipOnDraw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();
            using var bitmap = ImageUtility.Crop(_bookVm.OriginalPageBitmap, _bookVm.SelectedBlock.Rectangle);
            e.Graphics.DrawImage(bitmap, CommentToolTipBorderWidth, CommentToolTipBorderWidth);
        }

        private void CommentToolTipOnPopup(object sender, PopupEventArgs e)
        {
            var selectedBlock = _bookVm.SelectedBlock;
            if (selectedBlock != null)
            {
                e.ToolTipSize = new Size(
                    selectedBlock.Rectangle.Width + CommentToolTipBorderWidth * 2, 
                    selectedBlock.Rectangle.Height + CommentToolTipBorderWidth * 2);
            }
        }

        private void PagePbOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = true;
        }

        private void PagePbOnKeyDown(object sender, KeyEventArgs args)
        {

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

        public void Bind(BookViewModel bookVm)
        {
            if (_bookVm != null)
            {
                _bookVm.ImageRendererChanged -= OnImageRendererChanged;
                _bookVm.SelectedBlockChanged -= OnSelectedBlockChanged;
                _bookVm.BlockAdded -= BlockAdded;
                _bookVm.BlockModified -= BlockModified;
                _bookVm.BlockRemoved -= BlockRemoved;
                _bookVm.KeyboardEvent -= OnBookVmKeyboardEvent;
            }

            _bookVm = bookVm;

            _bookVm.ImageRendererChanged += OnImageRendererChanged;
            _bookVm.SelectedBlockChanged += OnSelectedBlockChanged;
            _bookVm.BlockAdded += BlockAdded;
            _bookVm.BlockModified += BlockModified;
            _bookVm.BlockRemoved += BlockRemoved;
            _bookVm.KeyboardEvent += OnBookVmKeyboardEvent;
        }

        private void OnBookVmKeyboardEvent(object sender, KeyboardArgs args)
        {
            if (KeyboardActions.ContainsKey(args.KeyValue))
            {
                KeyboardActions[args.KeyValue].Invoke(args);
            }
        }

        private void BlockRemoved(object sender, Block e)
        {
            page_pb.Refresh();
        }

        private void BlockModified(object sender, Block e)
        {
            if (_bookVm.GetBlockPage(e) == _bookVm.CurrentPage)
            {
                page_pb.Refresh();
                ShowHideCommentLinkToolTip();
            }
        }

        private void BlockAdded(object sender, Block e)
        {
            if (_bookVm.GetBlockPage(e) == _bookVm.CurrentPage)
            {
                page_pb.Refresh();
            }
        }

        private void OnSelectedBlockChanged(object sender, Block e)
        {
            page_pb.Image = _bookVm.OriginalPageBitmap;
            page_pb.Refresh();
            ShowHideCommentLinkToolTip();
        }

        private void ShowHideCommentLinkToolTip()
        {
            if (_bookVm.SelectedBlock is CommentLinkBlock commentLinkBlock)
            {
                var topRightCorner = page_pb.ToPictureBoxPoint(new Point(commentLinkBlock.BottomRightX, commentLinkBlock.TopLeftY));
                var toolTipLocation = new Point(topRightCorner.X, topRightCorner.Y - commentLinkBlock.Rectangle.Height);
                _commentToolTip.Show("c", page_pb, toolTipLocation, CommentToolTipDuration);
            }
            else
            {
                _commentToolTip.Hide(page_pb);
            }
        }

        private void OnImageRendererChanged(object sender, IImageRenderer e)
        {
            page_pb.Refresh();
        }

        private void PagePbOnMouseMove(object sender, MouseEventArgs e)
        {
            if (page_pb.Image == null) return;

            _bookVm.OriginalMouseAt = page_pb.ToOriginalPoint(e.Location);
            _bookVm.PictureBoxMouseAt = e.Location;

            if (e.Button == MouseButtons.Right && _bookVm.OriginalSelectionStartPoint.HasValue)
            {
                page_pb.Refresh();
            }

            if (_dragActivity != null)
            {
                _dragActivity.Perform(this, page_pb, e);
            }
        }

        private void PagePbOnMouseUp(object sender, MouseEventArgs e)
        {
            if (_bookVm.CurrentPage == null) return;

            _dragActivity = null;

            if (e.Button == MouseButtons.Right && _bookVm.OriginalSelectionStartPoint.HasValue)
            {
                var originalLocation = page_pb.ToOriginalPoint(e.Location);

                var xs = new List<int>
                    {
                        _bookVm.OriginalSelectionStartPoint.Value.X,
                        originalLocation.X
                    }
                    .OrderBy(i => i).ToList();

                var ys = new List<int>
                    {
                        _bookVm.OriginalSelectionStartPoint.Value.Y,
                        originalLocation.Y
                    }
                    .OrderBy(i => i).ToList();

                var originalRect = new Rectangle(xs[0], ys[0], xs[1] - xs[0], ys[1] - ys[0]);
                var menu = GetPageMenu(_bookVm.CurrentPage, originalRect);
                menu.Show(page_pb, e.Location);
                _bookVm.OriginalSelectionStartPoint = null;
                _bookVm.PbSelectionStartPoint = null;
            }
        }

        private ContextMenuStrip GetPageMenu(Page page, Rectangle originalRect)
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Recognize Text", null, async (o, a) =>
            {
                using var ocrBitmap = ImageUtility.Crop(_bookVm.OriginalPageBitmap, originalRect);
                using var ocrStream = new MemoryStream();
                ocrBitmap.Save(ocrStream, ImageFormat.Jpeg);
                var ocrPage = await _bookVm.OcrUtility.GetPageAsync(ocrStream.ToArray());
                Clipboard.SetText(ocrPage.GetText());
                _bookVm.SendInfo(this, "Copied text to clipboard");
            });

            menu.Items.Add("Add Image Block", null,
                (o, a) => _bookVm.AddBlock(this, ImageBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Title Block", null,
                (o, a) => _bookVm.AddBlock(this, TitleBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Comment Link Block", null,
                (o, a) => _bookVm.AddBlock(this, CommentLinkBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Garbage Block", null,
                (o, a) => _bookVm.AddBlock(this, GarbageBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            return menu;
        }

        private void PagePbOnMouseDown(object sender, MouseEventArgs e)
        {
            page_pb.Focus();

            if (page_pb.Image == null) return;

            if (e.Button == MouseButtons.Right)
            {
                _bookVm.OriginalSelectionStartPoint = page_pb.ToOriginalPoint(e.Location);
                _bookVm.PbSelectionStartPoint = e.Location;
            }

            if (e.Button == MouseButtons.Left)
            {
                var originalPoint = page_pb.ToOriginalPoint(e.Location);
                
                if (_bookVm.SelectedBlock != null)
                {
                    var dragPointLabel = DragPointLabelResolver.GetDragLabelAtPoint(_bookVm.SelectedBlock, page_pb, e.Location);
                    if (dragPointLabel.HasValue)
                    {
                        _dragActivity = DragActivityFactory.ConstructDragActivity(_bookVm, _bookVm.SelectedBlock, dragPointLabel.Value);
                        return;
                    }
                }

                _dragActivity = null;
                var blockAtCursor = _bookVm.CurrentPage.GetBlockAtPoint(originalPoint);
                _bookVm.SetBlockSelected(this, blockAtCursor);
            }
        }

        private void PagePbOnPaint(object sender, PaintEventArgs e)
        {
            if (_bookVm?.ImageRenderer != null)
            {
                _bookVm.ImageRenderer.Render(_bookVm.OriginalPageBitmap, e.Graphics);
            }
        }
    }
}
