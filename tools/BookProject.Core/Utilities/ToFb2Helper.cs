using System.Collections.Generic;
using System.Linq;
using BookProject.Core.Models.Domain;
using BookProject.Core.Models.ToFb2.PageElements;

namespace BookProject.Core.Utilities
{
    public static class ToFb2Helper
    {
        public static void TransferOverlappingParagraphs(Book book)
        {
            var domainPages = book.Pages.Where(p => p.Lines?.Any() == true).OrderBy(p => p.Index).ToArray();
            for (var i = 1; i < domainPages.Length; i++)
            {
                TransferOverlappingParagraphs(domainPages[i], domainPages[i - 1]);
            }
        }

        public static IEnumerable<ToFb2Paragraph> ConstructToFb2Paragraphs(Page page)
        {
            // paragraph blocks

            // block (word, comment link) - line index



            var blockGroups = new List<List<Block>>();




            yield break;
        }

        private static void TransferOverlappingParagraphs(Page fromPage, Page toPage)
        {
            if (fromPage.Lines?.Any() != true)
            {
                return;
            }

            var firstLineStart = fromPage.Lines.Where(l => l.Type == LineType.First).OrderByDescending(l => l.TopLeftY)
                .FirstOrDefault();

            var transferThreshold = firstLineStart?.TopLeftY ?? fromPage.BottomRightY;

            var linesToTransfer = fromPage.Lines.Where(l => l.TopLeftY < transferThreshold).ToArray();

            if (!linesToTransfer.Any())
            {
                return;
            }

            foreach (var line in linesToTransfer)
            {
                TransferOverlappingLine(line, fromPage, toPage);
            }
        }

        private static void TransferOverlappingLine(Line line, Page fromPage, Page toPage)
        {
            var commentLinkBlocks = fromPage.CommentLinkBlocks.Where(b => b.Rectangle.IntersectsWith(line.Rectangle));

            foreach (var commentLinkBlock in commentLinkBlocks)
            {
                fromPage.CommentLinkBlocks.Remove(commentLinkBlock);
                toPage.CommentLinkBlocks.Add(commentLinkBlock);
                commentLinkBlock.TopLeftY += toPage.BottomRightY;
                commentLinkBlock.BottomRightY += toPage.BottomRightY;
            }

            fromPage.Lines.Remove(line);
            toPage.Lines.Add(line);
            line.TopLeftY += toPage.BottomRightY;
            line.BottomRightY += toPage.BottomRightY;

            if (!line.Replace)
            {
                foreach (var word in line.Words)
                {
                    word.TopLeftY += toPage.BottomRightY;
                    word.BottomRightY += toPage.BottomRightY;
                }
            }
        }
    }
}