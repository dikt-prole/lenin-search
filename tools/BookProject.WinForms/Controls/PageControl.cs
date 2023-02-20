using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Utilities;
using BookProject.WinForms.DragActivities;
using BookProject.WinForms.PageActions;

namespace BookProject.WinForms.Controls
{
    public partial class PageControl : UserControl
    {
        private IDragActivity _dragActivity;
        private BookViewModel _bookVm;
        private readonly PreviewKeyDownPageActionFactory
            _previewKeyDownPageActionFactory = new PreviewKeyDownPageActionFactory();
        public PageControl()
        {
            InitializeComponent();
            page_pb.Paint += PagePbOnPaint;
            page_pb.MouseDown += PagePbOnMouseDown;
            page_pb.MouseUp += PagePbOnMouseUp;
            page_pb.MouseMove += PagePbOnMouseMove;
            page_pb.PreviewKeyDown += PagePbOnPreviewKeyDown;
        }

        private void PagePbOnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            var pageAction = _previewKeyDownPageActionFactory.Construct(e);
            if (pageAction != null)
            {
                e.IsInputKey = true;
                pageAction.Execute(this, _bookVm);
            }
        }

        public void Bind(BookViewModel bookVm)
        {
            if (_bookVm != null)
            {
                _bookVm.ImageRendererChanged -= OnImageRendererChanged;
                _bookVm.SelectedBlockChanged -= OnSelectedBlockChanged;
            }

            _bookVm = bookVm;

            _bookVm.ImageRendererChanged += OnImageRendererChanged;
            _bookVm.SelectedBlockChanged += OnSelectedBlockChanged;
            _bookVm.BlockAdded += BlockAdded;
            _bookVm.BlockModified += BlockModified;
            _bookVm.BlockRemoved += BlockRemoved;
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

            menu.Items.Add("Add Image Block", null,
                (o, a) => _bookVm.AddBlock(this, ImageBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Title Block", null,
                (o, a) => _bookVm.AddBlock(this, TitleBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Comment Link Block", null,
                (o, a) => _bookVm.AddBlock(this, CommentLinkBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Add Garbage Block", null,
                (o, a) => _bookVm.AddBlock(this, GarbageBlock.FromRectangle(originalRect), _bookVm.CurrentPage));

            menu.Items.Add("Recognize Text", null, async (o, a) =>
            {
                using var ocrBitmap = ImageUtility.Crop(_bookVm.OriginalPageBitmap, originalRect);
                using var ocrStream = new MemoryStream();
                ocrBitmap.Save(ocrStream, ImageFormat.Jpeg);
                var ocrPage = await _bookVm.OcrUtility.GetPageAsync(ocrStream.ToArray());
                Clipboard.SetText(ocrPage.GetText());
                MessageBox.Show("Text copied to clipboard", "Recognize Text", MessageBoxButtons.OK, MessageBoxIcon.Information);
            });

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
