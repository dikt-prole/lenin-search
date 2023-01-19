using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LeninSearch.Studio.WinForms.Model;
using LeninSearch.Studio.WinForms.Model.Fb2;

namespace LeninSearch.Studio.WinForms.Service
{
    public class Fb2Service : IFb2Service
    {
        public void GenerateFb2File(string ocrFolder, string fb2File, string fb2Template)
        {
            // 1. load ocr data
            var ocrData = OcrData.Load(ocrFolder);

            var ocrPages = ocrData.Pages.OrderBy(p => p.ImageIndex).ToList();

            // 2. massage ocr data
            var emptyLinks = FixCommentRelatedStuff(ocrData);
            FixTextRelatedStuff(ocrData);

            // 3. go through pages and create fb2 lines (paragraphs, titles, images)
            var fb2Lines = new List<Fb2Line>();
            var imageIndex = 1;
            for (var pageIndex = 0; pageIndex < ocrPages.Count; pageIndex++)
            {
                var page = ocrPages[pageIndex];

                var textLinesFb2 = new List<Fb2Line>();
                var ocrTextLines = page.Lines.Where(l => l.Label == OcrLabel.PStart || l.Label == OcrLabel.PMiddle).ToList();
                if (ocrTextLines.Any())
                {
                    var currentParagraphLineFb2 = Fb2Line.Construct(ocrTextLines[0]);
                    textLinesFb2.Add(currentParagraphLineFb2);

                    for (var paragraphOcrLineIndex = 1; paragraphOcrLineIndex < ocrTextLines.Count; paragraphOcrLineIndex++)
                    {
                        var currentLine = ocrTextLines[paragraphOcrLineIndex];

                        if (currentLine.Label == OcrLabel.PStart)
                        {
                            currentParagraphLineFb2 = Fb2Line.Construct(currentLine);
                            textLinesFb2.Add(currentParagraphLineFb2);
                        }
                        else
                        {
                            currentParagraphLineFb2.Lines.Add(currentLine);
                        }
                    }
                }

                var imageFb2Lines = new List<Fb2Line>();
                foreach (var imageBlock in page.ImageBlocks)
                {
                    var imageBlockFb2 = Fb2Line.Construct(imageBlock, ocrFolder, page, imageIndex);
                    imageFb2Lines.Add(imageBlockFb2);
                    imageIndex++;
                }

                var titleFb2Lines = new List<Fb2Line>();
                foreach (var titleBlock in page.TitleBlocks ?? new List<OcrTitleBlock>())
                {
                    titleFb2Lines.Add(Fb2Line.Construct(titleBlock));
                }

                var lines = textLinesFb2
                    .Concat(imageFb2Lines)
                    .Concat(titleFb2Lines)
                    .OrderBy(l => l.TopLeftY).ToList();

                fb2Lines.AddRange(lines);
            }

            // 4. construct fb2 tags
            var topmostTag = new Fb2Tag
            {
                TitleLevel = -1,
                Children = new List<Fb2Tag>()
            };
            var parentTag = topmostTag;
            foreach (var fb2Line in fb2Lines)
            {
                if (fb2Line.Type == Fb2LineType.Title)
                {
                    var titleTagParent = parentTag;
                    while (titleTagParent.TitleLevel >= fb2Line.TitleLevel)
                    {
                        titleTagParent = titleTagParent.Parent;
                    }

                    var titleTag = new Fb2Tag
                    {
                        TitleLevel = fb2Line.TitleLevel,
                        Fb2Line = fb2Line,
                        Children = new List<Fb2Tag>(),
                        Parent = titleTagParent
                    };
                    titleTagParent.Children.Add(titleTag);
                    parentTag = titleTag;
                }
                else
                {
                    parentTag.Children.Add(new Fb2Tag { Fb2Line = fb2Line });
                }
            }

            // 4. construct body xml
            var tagsSb = new StringBuilder();
            foreach (var tag in topmostTag.Children)
            {
                tagsSb.Append(tag.GetXml());
                tagsSb.Append(Environment.NewLine);
            }
            var bodyXml = tagsSb.ToString();

            // 5. go through pages and create notes
            var notes = new List<Fb2Line>();
            foreach (var page in ocrPages)
            {
                var commentLines = page.GetLabeledLines(OcrLabel.Comment).ToList();
                if (commentLines.Any())
                {
                    var currentCommentLineFb2 = Fb2Line.Construct(commentLines[0]);
                    notes.Add(currentCommentLineFb2);
                    for (var commentLineIndex = 1; commentLineIndex < commentLines.Count; commentLineIndex++)
                    {
                        var commentLine = commentLines[commentLineIndex];
                        var commentLinkWord = commentLine.Words.FirstOrDefault(w => w.IsCommentLinkNumber);
                        if (commentLinkWord != null)
                        {
                            currentCommentLineFb2 = Fb2Line.Construct(commentLine);
                            notes.Add(currentCommentLineFb2);
                        }
                        else
                        {
                            currentCommentLineFb2.Lines.Add(commentLine);
                        }
                    }
                }
            }

            // 6. construct body-notes
            var notesSb = new StringBuilder();
            foreach (var note in notes)
            {
                if (note.Lines.Any(l => l.Words.Any(w => emptyLinks.Contains(w)))) continue;

                var sectionXml = note.GetXml();
                notesSb.Append(sectionXml);
                notesSb.Append(Environment.NewLine);
            }
            var bodyNotesXml = notesSb.ToString();

            // 7. construct binary content
            var imageLines = fb2Lines.Where(l => l.Type == Fb2LineType.Image).ToList();
            var imagesSb = new StringBuilder();
            foreach (var imageLine in imageLines)
            {
                var binaryContentItemXml = $"<binary content-type=\"image/jpeg\" id=\"i_{imageLine.ImageIndex}.jpg\">{imageLine.ImageBase64}</binary>";
                imagesSb.Append(binaryContentItemXml);
                imagesSb.Append(Environment.NewLine);
            }
            var binaryContentXml = imagesSb.ToString();

            // 8. load and fill template
            var fb2Xml = fb2Template
                .Replace(Tokens.Body, bodyXml)
                .Replace(Tokens.BodyNotes, bodyNotesXml)
                .Replace(Tokens.BinaryContent, binaryContentXml);

            // 9. write fb2 file
            File.WriteAllText(fb2File, fb2Xml);
        }

        private static List<OcrWord> FixCommentRelatedStuff(OcrData ocrData)
        {
            var emptyLinks = new List<OcrWord>();

            var ocrPages = ocrData.Pages.OrderBy(p => p.ImageIndex).ToList();

            // 1. throughout note labeling (1-100, not 1-2 on each page)
            var processedLinkedTextWords = new List<OcrWord>();
            for (var pageIndex = 1; pageIndex < ocrPages.Count; pageIndex++)
            {
                var page = ocrPages[pageIndex];
                var commentLines = page.GetLabeledLines(OcrLabel.Comment).ToList();
                var textLines = page.GetLabeledLines(OcrLabel.PStart, OcrLabel.PMiddle).ToList();

                var titleBlockLinkedTextWords = page.TitleBlocks == null
                    ? new List<OcrWord>()
                    : page.TitleBlocks.Where(tb => tb.CommentLinks != null).SelectMany(tb => tb.CommentLinks);

                var linkedTextWords = textLines
                    .SelectMany(l => l.Words)
                    .Concat(titleBlockLinkedTextWords)
                    .Where(w => w.IsCommentLinkNumber)
                    .ToList();

                foreach (var commentLine in commentLines)
                {
                    var linkedCommentWord = commentLine.Words.FirstOrDefault(w => w.IsCommentLinkNumber);
                    if (linkedCommentWord == null) continue;

                    var linkedTextWord = linkedTextWords.FirstOrDefault(w =>
                        w.Text == linkedCommentWord.Text && !processedLinkedTextWords.Contains(w));

                    if (linkedTextWord == null)
                    {
                        emptyLinks.Add(linkedCommentWord);
                        continue;
                    }

                    var linkText = (processedLinkedTextWords.Count + 1).ToString();
                    linkedCommentWord.Text = linkText;
                    linkedTextWord.Text = linkText;
                    processedLinkedTextWords.Add(linkedTextWord);
                }
            }

            // 2. comment should start and end on the same page
            for (var pageIndex = 1; pageIndex < ocrPages.Count; pageIndex++)
            {
                var middleLines = ocrPages[pageIndex].GetLabeledLines(OcrLabel.Comment)
                    .TakeWhile(l => l.Words.All(w => !w.IsCommentLinkNumber))
                    .ToList();

                if (!middleLines.Any()) continue;

                for (var reversePageIndex = pageIndex - 1; reversePageIndex >= 0; reversePageIndex--)
                {
                    if (ocrPages[reversePageIndex].GetLabeledLines(OcrLabel.Comment).Any())
                    {
                        foreach (var middleLine in middleLines)
                        {
                            ocrPages[pageIndex].Lines.Remove(middleLine);
                            ocrPages[reversePageIndex].Lines.Add(middleLine);
                        }

                        break;
                    }
                }
            }

            return emptyLinks;
        }

        private static void FixTextRelatedStuff(OcrData ocrData)
        {
            var ocrPages = ocrData.Pages.OrderBy(p => p.ImageIndex).ToList();

            // 1. paragraph should start and end on the same page
            for (var pageIndex = 1; pageIndex < ocrPages.Count; pageIndex++)
            {
                var middleLines = ocrPages[pageIndex].Lines
                    .Where(l => l.Label == OcrLabel.PStart || l.Label == OcrLabel.PMiddle)
                    .TakeWhile(l => l.Label == OcrLabel.PMiddle)
                    .ToList();

                if (!middleLines.Any()) continue;

                for (var reversePageIndex = pageIndex - 1; reversePageIndex >= 0; reversePageIndex--)
                {
                    if (ocrPages[reversePageIndex].Lines.Any(l => l.Label == OcrLabel.PStart))
                    {
                        foreach (var middleLine in middleLines)
                        {
                            ocrPages[pageIndex].Lines.Remove(middleLine);
                            ocrPages[reversePageIndex].Lines.Add(middleLine);
                        }

                        break;
                    }
                }
            }
        }

        private static class Tokens
        {
            public const string Body = "[body]";
            public const string BodyNotes = "[body-notes]";
            public const string BinaryContent = "[binary-content]";
        }
    }
}