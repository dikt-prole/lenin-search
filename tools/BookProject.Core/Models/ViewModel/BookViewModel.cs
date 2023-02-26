using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using BookProject.Core.ImageRendering;
using BookProject.Core.Models.Domain;
using BookProject.Core.Settings;
using BookProject.Core.Utilities;

namespace BookProject.Core.Models.ViewModel
{
    public class BookViewModel
    {
        public EventHandler<Block> SelectedBlockChanged;

        public EventHandler<Block> BlockAdded;

        public EventHandler<Block> BlockRemoved;

        public EventHandler<Block> BlockModified;

        public EventHandler<IImageRenderer> ImageRendererChanged;

        public EventHandler<BookProjectSettings> SettingsChanged;

        public EventHandler<KeyboardArgs> KeyboardEvent;

        public Book Book { get; set; }
        public Page CurrentPage => GetBlockPage(SelectedBlock);
        public Point? OriginalSelectionStartPoint { get; set; }
        public Point? PbSelectionStartPoint { get; set; }
        public Point OriginalMouseAt { get; set; }
        public Point PictureBoxMouseAt { get; set; }
        public Bitmap OriginalPageBitmap { get; private set; }
        public Block SelectedBlock { get; private set; }
        public IImageRenderer ImageRenderer { get; private set; }
        public BookProjectSettings Settings { get; private set; }

        public IOcrUtility OcrUtility { get; } = new YandexVisionOcrUtility();

        public void SetImageRenderer(object sender, IImageRenderer imageRenderer)
        {
            ImageRenderer = imageRenderer;
            ImageRendererChanged?.Invoke(sender, imageRenderer);
        }

        public void SetAndSaveDetectImageSettings(object sender, DetectImageSettings settings)
        {
            Settings.ImageDetection = settings;
            Settings.Save();
            SettingsChanged?.Invoke(sender, Settings);
        }

        public void SetAndSaveDetectTitleSettings(object sender, DetectTitleSettings settings)
        {
            Settings.TitleDetection = settings;
            Settings.Save();
            SettingsChanged?.Invoke(sender, Settings);
        }

        public void SetAndSaveDetectGarbageSettings(object sender, DetectGarbageSettings settings)
        {
            Settings.GarbageDetection = settings;
            Settings.Save();
            SettingsChanged?.Invoke(sender, Settings);
        }

        public void SetAndSaveDetectCommentLinkSettings(object sender, DetectCommentLinkSettings settings)
        {
            Settings.CommentLinkDetection = settings;
            Settings.Save();
            SettingsChanged?.Invoke(sender, Settings);
        }

        public void AddBlock(object sender, Block block, Page page, bool setEdit = true)
        {
            if (block is ImageBlock imageBlock)
            {
                page.ImageBlocks ??= new List<ImageBlock>();
                page.ImageBlocks.Add(imageBlock);
            }
            else if (block is TitleBlock titleBlock)
            {
                page.TitleBlocks ??= new List<TitleBlock>();
                page.TitleBlocks.Add(titleBlock);
            }
            else if (block is CommentLinkBlock commentLinkBlock)
            {
                page.CommentLinkBlocks ??= new List<CommentLinkBlock>();
                page.CommentLinkBlocks.Add(commentLinkBlock);
            }
            else if (block is GarbageBlock garbageBlock)
            {
                page.GarbageBlocks ??= new List<GarbageBlock>();
                page.GarbageBlocks.Add(garbageBlock);
            }
            else if (block is Line line)
            {
                page.Lines ??= new List<Line>();
                page.Lines.Add(line);
            }

            BlockAdded?.Invoke(sender, block);
            if (setEdit)
            {
                SetBlockSelected(sender, block);
            }
        }

        public void RemoveBlock(object sender, Block block)
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

            if (SelectedBlock == block)
            {
                SetBlockSelected(sender, page);
            }
            BlockRemoved?.Invoke(sender, block);
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
            Block[] removeBlocks = null;

            if (typeof(TBlock) == typeof(ImageBlock))
            {
                removeBlocks = page.ImageBlocks.ToArray();
            }

            if (typeof(TBlock) == typeof(TitleBlock))
            {
                removeBlocks = page.TitleBlocks.ToArray();
            }

            if (typeof(TBlock) == typeof(CommentLinkBlock))
            {
                removeBlocks = page.CommentLinkBlocks.ToArray();
            }

            if (typeof(TBlock) == typeof(GarbageBlock))
            {
                removeBlocks = page.GarbageBlocks.ToArray();
            }

            if (removeBlocks != null)
            {
                foreach (var b in removeBlocks)
                {
                    RemoveBlock(sender, b);
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
            ImageRenderer = new PageStateRenderer(this);
            SelectedBlockChanged?.Invoke(sender, block);
        }

        public void SelectNextCurrentPageBlock(object sender)
        {
            var blocks = CurrentPage.GetNonPageBlocks().OrderBy(b => b.TopLeftY).ThenBy(b => b.TopLeftX).ToList();

            if (SelectedBlock == CurrentPage)
            {
                if (blocks.Count > 0)
                {
                    SetBlockSelected(sender, blocks[0]);
                }
            }
            else if (blocks.Count > 0)
            {
                var editIndex = blocks.FindIndex(b => b == SelectedBlock);

                if (editIndex == -1)
                {
                    SetBlockSelected(sender, blocks[0]);
                    return;
                }

                var nextEditIndex = (editIndex + 1) % blocks.Count;

                SetBlockSelected(sender, blocks[nextEditIndex]);
            }
        }

        public Page GetBlockPage(Block block)
        {
            if (block == null) return null;

            return Book.Pages?.FirstOrDefault(p => p.GetAllBlocks().Contains(block));
        }

        public void RegisterKeyboardEvent(object sender, KeyboardArgs args)
        {
            KeyboardEvent?.Invoke(sender, args);
        }

        public static BookViewModel Initialize(string bookFolder)
        {
            var book = Book.Load(bookFolder);
            var settings = BookProjectSettings.Load();
            return new BookViewModel
            {
                Book = book,
                Settings = settings
            };
        }

        public void SetAndSaveDetectLineSettings(object sender, DetectLineSettings settings)
        {
            Settings.LineDetection = settings;
            Settings.Save();
            SettingsChanged?.Invoke(sender, Settings);
        }
    }
}