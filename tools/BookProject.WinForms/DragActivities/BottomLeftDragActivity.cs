﻿using System.Windows.Forms;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ViewModel;

namespace BookProject.WinForms.DragActivities
{
    public class BottomLeftDragActivity : IDragActivity
    {
        private readonly BookViewModel _bookVm;
        private readonly Block _block;
        public BottomLeftDragActivity(BookViewModel bookVm, Block block)
        {
            _bookVm = bookVm;
            _block = block;
        }

        public void Perform(object sender, PictureBox pictureBox, MouseEventArgs args)
        {
            _bookVm.ModifyBlock(sender, _block, b =>
            {
                var originalPoint = pictureBox.ToOriginalPoint(args.Location);
                b.TopLeftX = originalPoint.X;
                b.BottomRightY = originalPoint.Y;
            });
        }
    }
}