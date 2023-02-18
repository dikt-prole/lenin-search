using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.Models.Domain;
using BookProject.Core.Utilities;

namespace BookProject.Core.Models.ViewModel
{
    public class BookViewModel
    {
        public EventHandler<Block> SelectedBlockChanged;

        public EventHandler<Block> BlockAdded;

        public EventHandler<Block> BlockRemoved;

        public EventHandler<Block> BlockModified;

        public Book Book { get; set; }
        public Page CurrentPage => GetBlockPage(SelectedBlock);
        public Point? OriginalSelectionStartPoint { get; set; }
        public Point? PbSelectionStartPoint { get; set; }
        public Point OriginalMouseAt { get; set; }
        public Point PictureBoxMouseAt { get; set; }
        public Bitmap OriginalPageBitmap { get; private set; }
        public Block SelectedBlock { get; private set; }

        public void AddBlock(object sender, Block block, Page page, bool setEdit = true)
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

            BlockAdded?.Invoke(sender, block);
            if (setEdit)
            {
                SetBlockSelected(sender, block);
            }
        }

        public void RemoveBlock(object sender, Block block, bool setEdit = true)
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

            BlockRemoved?.Invoke(sender, block);
            if (setEdit)
            {
                SetNextEditBlock(sender, page);
            }
        }

        public void ModifyBlock<TBlock>(object sender, TBlock block, Action<TBlock> modifyAction)  where TBlock : Block
        {
            var page = Book.Pages.FirstOrDefault(p => p.GetAllBlocks().Count(b => b == block) > 0);

            if (page == null) return;

            modifyAction(block);

            BlockModified?.Invoke(sender, block);
        }

        public void SetPageBlocks<TBlock>(object sender, Page page, IEnumerable<TBlock> blocks) where TBlock : Block
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
                    RemoveBlock(sender, b, false);
                }
            }

            foreach (var block in blocks)
            {
                AddBlock(sender, block, page);
            }
        }

        public void SetBlockSelected(object sender, Block block)
        {
            SelectedBlock = block;
            var page = GetBlockPage(block);
            var imageFile = Path.Combine(Book.Folder, $"{page.ImageFile}.jpg");
            OriginalPageBitmap = ImageUtility.Load(imageFile);
            SelectedBlockChanged?.Invoke(sender, block);
        }

        public void SetNextEditBlock(object sender, Page page)
        {
            var blocks = page.GetAllBlocks().OrderBy(b => b.TopLeftY).ThenBy(b => b.TopLeftX).ToList();

            if (blocks.Count == 0) return;

            var editIndex = blocks.FindIndex(b => b == SelectedBlock);

            if (editIndex == -1)
            {
                SetBlockSelected(sender, blocks[0]);
                return;
            }

            var nextEditIndex = (editIndex + 1) % blocks.Count;

            SetBlockSelected(sender, blocks[nextEditIndex]);
        }

        public Page GetBlockPage(Block block)
        {
            if (block == null) return null;

            return Book.Pages?.FirstOrDefault(p => p.GetAllBlocks().Contains(block));
        }
    }
}