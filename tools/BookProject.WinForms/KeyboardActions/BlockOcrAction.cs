using System;
using System.Threading.Tasks;
using BookProject.Core.Models;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;
using BookProject.Core.Utilities;

namespace BookProject.WinForms.KeyboardActions
{
    public class BlockOcrAction : IKeyboardAction
    {
        public void Execute(object sender, BookViewModel bookVm, KeyboardArgs args)
        {
            if (!args.Control) return;

            var selectedBlock = bookVm.SelectedBlock;

            if (selectedBlock is Line line)
            {
                try
                {
                    using var lineBitmap = ImageUtility.Crop(bookVm.OriginalPageBitmap, line.Rectangle);
                    var lineBytes = ImageUtility.GetJpegBytes(lineBitmap);
                    var ocrPage = Task.Run(() => bookVm.OcrUtility.GetPageAsync(lineBytes)).Result;
                    bookVm.ModifyBlock(sender, line, l =>
                    {
                        l.Replace = true;
                        l.ReplaceText = ocrPage.GetText();
                    });
                    bookVm.SendInfo(sender, "Recognized line text");
                }
                catch (Exception exc)
                {
                    bookVm.SendError(sender, exc.Message);
                }
            }

            if (selectedBlock is TitleBlock titleBlock)
            {
                try
                {
                    using var titleBitmap = ImageUtility.Crop(bookVm.OriginalPageBitmap, titleBlock.Rectangle);
                    var titleBytes = ImageUtility.GetJpegBytes(titleBitmap);
                    var ocrPage = Task.Run(() => bookVm.OcrUtility.GetPageAsync(titleBytes)).Result;
                    bookVm.ModifyBlock(sender, titleBlock, tb =>
                    {
                        tb.Text = ocrPage.GetText();
                    });
                    bookVm.SendInfo(sender, "Recognized title text");
                }
                catch (Exception exc)
                {
                    bookVm.SendError(sender, exc.Message);
                }
            }
        }
    }
}