using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BookProject.Core.Models.Domain;

namespace BookProject.Core.Models.ViewModel
{
    public class BookViewModel
    {
        public EventHandler<BlockEventArgs> EditBlockSelectionChanged;

        public EventHandler<BlockEventArgs> BlockAdded;

        public EventHandler<BlockEventArgs> BlockRemoved;

        public EventHandler<BlockEventArgs> BlockModified;

        public Book Book { get; set; }
        public Page CurrentPage { get; set; }
        public Point? OriginalSelectionStartPoint { get; set; }
        public Point? PbSelectionStartPoint { get; set; }
        public Point OriginalMouseAt { get; set; }
        public Point PictureBoxMouseAt { get; set; }
        public Bitmap OriginalPageBitmap { get; set; }

        public void AddBlock(Block block, Page page, bool setEdit = true)
        {
            if (block is ImageBlock imageBlock)
            {
                page.ImageBlocks.Add(imageBlock);
            }
            else if (block is TitleBlock titleBlock)
            {
                page.TitleBlocks.Add(titleBlock);
            }
            else if (block is CommentLinkBlock commentLinkBlock)
            {
                page.CommentLinkBlocks.Add(commentLinkBlock);
            }
            else if (block is GarbageBlock garbageBlock)
            {
                page.GarbageBlocks.Add(garbageBlock);
            }
            else if (block is Line line)
            {
                page.Lines.Add(line);
            }

            BlockAdded?.Invoke(this, new BlockEventArgs(page, block));
            if (setEdit)
            {
                SetEditBlock(block, page);
            }
        }

        public void RemoveBlock(Block block, bool setEdit = true)
        {
            var page = Book.Pages.FirstOrDefault(p => p.GetAllBlocks().Count(b => b == block) > 0);

            if (page == null) return;

            if (block is ImageBlock imageBlock)
            {
                page.ImageBlocks.Remove(imageBlock);
            }
            else if (block is TitleBlock titleBlock)
            {
                page.TitleBlocks.Remove(titleBlock);
            }
            else if (block is CommentLinkBlock commentLinkBlock)
            {
                page.CommentLinkBlocks.Remove(commentLinkBlock);
            }
            else if (block is GarbageBlock garbageBlock)
            {
                page.GarbageBlocks.Remove(garbageBlock);
            }
            else if (block is Line line)
            {
                page.Lines.Remove(line);
            }

            BlockRemoved?.Invoke(this, new BlockEventArgs(page, block));
            if (setEdit)
            {
                SetNextEditBlock(page);
            }
        }

        public void ModifyBlock<TBlock>(TBlock block, Action<TBlock> modifyAction)  where TBlock : Block
        {
            var page = Book.Pages.FirstOrDefault(p => p.GetAllBlocks().Count(b => b == block) > 0);

            if (page == null) return;

            modifyAction(block);

            BlockModified?.Invoke(this, new BlockEventArgs(page, block));
        }

        public void SetPageBlocks<TBlock>(Page page, IEnumerable<TBlock> blocks) where TBlock : Block
        {
            IEnumerable<Block> removeBlocks = null;

            if (typeof(TBlock) == typeof(ImageBlock))
            {
                removeBlocks = page.ImageBlocks;
            }

            if (typeof(TBlock) == typeof(TitleBlock))
            {
                removeBlocks = page.TitleBlocks;
            }

            if (typeof(TBlock) == typeof(CommentLinkBlock))
            {
                removeBlocks = page.CommentLinkBlocks;
            }

            if (typeof(TBlock) == typeof(GarbageBlock))
            {
                removeBlocks = page.GarbageBlocks;
            }

            if (removeBlocks != null)
            {
                foreach (var b in removeBlocks)
                {
                    RemoveBlock(b, false);
                }
            }

            foreach (var block in blocks)
            {
                AddBlock(block, page);
            }
        }

        public void SetEditBlock(Block block, Page page)
        {
            foreach (var b in page.GetAllBlocks())
            {
                b.State = b == block ? BlockState.Edit : BlockState.Normal;
            }

            EditBlockSelectionChanged?.Invoke(this, new BlockEventArgs(page, block));
        }

        public void SetNextEditBlock(Page page)
        {
            var blocks = page.GetAllBlocks().OrderBy(b => b.TopLeftY).ThenBy(b => b.TopLeftX).ToList();

            if (blocks.Count == 0) return;

            var editIndex = blocks.FindIndex(b => b.State == BlockState.Edit);

            if (editIndex == -1)
            {
                SetEditBlock(blocks[0], page);
                return;
            }

            var nextEditIndex = (editIndex + 1) % blocks.Count;

            SetEditBlock(blocks[nextEditIndex], page);
        }
    }
}