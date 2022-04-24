using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LeninSearch.Ocr.Model;
using LeninSearch.Script.Scripts.Models;

namespace LeninSearch.Script.Scripts
{
    public class OcrToFb2Script : IScript
    {
        public string Id => "ocr-to-fb2";
        public string Arguments => "ocr-book-folder, fb2-path";

        public void Execute(params string[] input)
        {
            //var ocrBookFolder = input[0];
            //var fb2Path = input[1];
            var ocrBookFolder = @"D:\Repo\lenin-search\corpus\ocr\pavlov-v1\t01";
            var fb2Path = @"D:\Repo\lenin-search\corpus\orig\pavlov\t01.fb2";

            // 1. load ocr data
            var ocrData = OcrData.Load(ocrBookFolder);

            // 2. massage ocr data

            // 2.1 throughout note labeling (1-100, not 1-2 on each page)
            var processedLinkedTextWords = new List<OcrWord>();
            for (var pageIndex = 1; pageIndex < ocrData.Pages.Count; pageIndex++)
            {
                var page = ocrData.Pages[pageIndex];
                var commentLines = page.GetLabeledLines(OcrLabel.Comment).ToList();
                var textLines = page.GetLabeledLines(OcrLabel.Title, OcrLabel.PStart, OcrLabel.PMiddle).ToList();

                foreach (var commentLine in commentLines)
                {
                    var linkedCommentWord = commentLine.Words.FirstOrDefault(w => w.IsCommentLinkNumber);
                    if (linkedCommentWord == null) continue;

                    var linkedTextWord = textLines.SelectMany(l => l.Words)
                        .FirstOrDefault(w => w.IsCommentLinkNumber &&
                                             w.Text == linkedCommentWord.Text &&
                                             !processedLinkedTextWords.Contains(w));
                    if (linkedTextWord == null) continue;

                    var linkText = (processedLinkedTextWords.Count + 1).ToString();
                    linkedCommentWord.Text = linkText;
                    linkedTextWord.Text = linkText;
                    processedLinkedTextWords.Add(linkedTextWord);
                }
            }

            // 2.2 paragraph should start and end on the same page
            for (var pageIndex = 1; pageIndex < ocrData.Pages.Count; pageIndex++)
            {
                var middleLines = ocrData.Pages[pageIndex].Lines
                    .Where(l => l.Label == OcrLabel.PStart || l.Label == OcrLabel.PMiddle)
                    .TakeWhile(l => l.Label == OcrLabel.PMiddle)
                    .ToList();

                if (!middleLines.Any()) continue;

                for (var reversePageIndex = pageIndex - 1; reversePageIndex >= 0; reversePageIndex--)
                {
                    if (ocrData.Pages[reversePageIndex].Lines.Any(l => l.Label == OcrLabel.PStart))
                    {
                        foreach (var middleLine in middleLines)
                        {
                            ocrData.Pages[pageIndex].Lines.Remove(middleLine);
                            ocrData.Pages[reversePageIndex].Lines.Add(middleLine);
                        }

                        break;
                    }
                }
            }

            // 2.3 multiline titles should be on single line
            foreach (var page in ocrData.Pages)
            {
                for (var lineIndex = 0; lineIndex < page.Lines.Count - 1; lineIndex++)
                {
                    if (page.Lines[lineIndex].Label == OcrLabel.Title &&
                        page.Lines[lineIndex + 1].Label == OcrLabel.Title)
                    {
                        page.Lines[lineIndex].Words.AddRange(page.Lines[lineIndex + 1].Words);
                        page.Lines.RemoveAt(lineIndex + 1);
                        lineIndex--;
                    }
                }
            }

            // 3. go through pages and create sections & images
            var sections = new List<List<Fb2Line>>();
            var imageIndex = 1;
            for (var pageIndex = 0; pageIndex < ocrData.Pages.Count; pageIndex++)
            {
                var page = ocrData.Pages[pageIndex];

                var pageParagraphLinesFb2 = new List<Fb2Line>();
                var ocrTextLines = page.Lines.Where(l => l.Label == OcrLabel.PStart || l.Label == OcrLabel.PMiddle || l.Label == OcrLabel.Title).ToList();
                if (ocrTextLines.Any())
                {
                    var currentParagraphLineFb2 = Fb2Line.Construct(ocrTextLines[0]);
                    pageParagraphLinesFb2.Add(currentParagraphLineFb2);

                    for (var paragraphOcrLineIndex = 1; paragraphOcrLineIndex < ocrTextLines.Count; paragraphOcrLineIndex++)
                    {
                        var currentLine = ocrTextLines[paragraphOcrLineIndex];

                        if (currentLine.Label == OcrLabel.PStart)
                        {
                            currentParagraphLineFb2 = Fb2Line.Construct(currentLine);
                            pageParagraphLinesFb2.Add(currentParagraphLineFb2);
                        }
                        else if (currentLine.Label == OcrLabel.Title)
                        {
                            pageParagraphLinesFb2.Add(Fb2Line.Construct(currentLine));
                            currentParagraphLineFb2 = null;
                        }
                        else
                        {
                            if (currentParagraphLineFb2 == null)
                                throw new Exception($"Page {pageIndex}: expected not null current paragraph line. Check that paragraph start goes after title");

                            currentParagraphLineFb2.Lines.Add(currentLine);
                        }
                    }
                }

                var pageImageLinesFb2 = new List<Fb2Line>();
                foreach (var imageBlock in page.ImageBlocks)
                {
                    var imageBlockFb2 = Fb2Line.Construct(imageBlock, ocrBookFolder, pageIndex, imageIndex);
                    pageImageLinesFb2.Add(imageBlockFb2);
                    imageIndex++;
                }

                var section = pageParagraphLinesFb2.Concat(pageImageLinesFb2).OrderBy(l => l.TopLeftY).ToList();
                sections.Add(section);
            }

            // 4. construct body xml
            var sectionsSb = new StringBuilder();
            for (var sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                var sectionXml =
                    $"<section idx=\"{sectionIndex}\">{Environment.NewLine}{string.Join(Environment.NewLine, sections[sectionIndex].Select(l => l.GetXml()))}{Environment.NewLine}</section>";
                sectionsSb.Append(sectionXml);
                sectionsSb.Append(Environment.NewLine);
            }
            var bodyXml = sectionsSb.ToString();

            // 5. go through pages and create notes
            var notes = new List<Fb2Line>();
            for (var pageIndex = 1; pageIndex < ocrData.Pages.Count; pageIndex++)
            {
                var page = ocrData.Pages[pageIndex];
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
                var sectionXml = note.GetXml();
                notesSb.Append(sectionXml);
                notesSb.Append(Environment.NewLine);
            }
            var bodyNotesXml = notesSb.ToString();

            // 7. construct binary content
            var imageLines = sections.SelectMany(s => s.Where(l => l.Type == Fb2LineType.Image)).ToList();
            var imagesSb = new StringBuilder();
            foreach (var imageLine in imageLines)
            {
                var binaryContentItemXml = $"<binary content-type=\"image/jpeg\" id=\"i_{imageLine.ImageIndex}.jpg\">{imageLine.ImageBase64}</binary>";
                imagesSb.Append(binaryContentItemXml);
                imagesSb.Append(Environment.NewLine);
            }
            var binaryContentXml = imagesSb.ToString();

            // 8. load and fill template
            var template = File.ReadAllText("fb2template.xml");
            var fb2Xml = template
                .Replace(Tokens.Body, bodyXml)
                .Replace(Tokens.BodyNotes, bodyNotesXml)
                .Replace(Tokens.BinaryContent, binaryContentXml);

            // 9. write fb2 file
            File.WriteAllText(fb2Path, fb2Xml);
        }

        private static class Tokens
        {
            public static class TitleInfo
            {
                public const string Genre = "[genre]";
                public const string BookAuthorFirstName = "[book-author-first-name]";
                public const string BookAuthorLastName = "[book-author-last-name]";
                public const string BookAuthorMiddleName = "[book-author-middle-name]";
                public const string BookId = "[book-id]";
                public const string BookTitle = "[book-title]";
                public const string BookAnnotation = "[book-annotation]";
                public const string BookLanguage = "[book-language]";
            }

            public static class DocumentInfo
            {
                public const string DocAuthorFirstName = "[doc-author-first-name]";
                public const string DocAuthorLastName = "[doc-author-last-name]";
                public const string DocId = "[doc-id]";
                public const string DocVersion = "[doc-version]";
            }

            public static string CommentMarker(string id, string text)
            {
                return $"<a l:href=\"#{id}\" type=\"note\">{text}</a>";
            }

            public const string Body = "[body]";

            public const string BodyNotes = "[body-notes]";

            public const string BinaryContent = "[binary-content]";
        }
    }


}