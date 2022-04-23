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

            // 2.1 paragraph should start and end on the same page
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

            // 2.2 multiline titles should be on single line
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

            // 2. go through pages and create sections, notes & images
            var sections = new List<List<Fb2Line>>();
            var notes = new List<string>();
            for (var pageIndex = 0; pageIndex < ocrData.Pages.Count; pageIndex++)
            {
                var page = ocrData.Pages[pageIndex];
                var pageParagraphLinesFb2 = new List<Fb2Line>();
                var pageImageLinesFb2 = new List<Fb2Line>();

                var paragraphOcrLines = page.Lines.Where(l => l.Label == OcrLabel.PStart || l.Label == OcrLabel.PMiddle || l.Label == OcrLabel.Title).ToList();
                if (paragraphOcrLines.Any())
                {
                    var currentParagraphLineFb2 = Fb2Line.Construct(paragraphOcrLines[0]);
                    pageParagraphLinesFb2.Add(currentParagraphLineFb2);

                    for (var paragraphOcrLineIndex = 1; paragraphOcrLineIndex < paragraphOcrLines.Count; paragraphOcrLineIndex++)
                    {
                        var currentLine = paragraphOcrLines[paragraphOcrLineIndex];

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

                var section = pageParagraphLinesFb2.Concat(pageImageLinesFb2).OrderBy(l => l.TopLeftY).ToList();
                sections.Add(section);
            }

            // 3. construct body xml
            var sectionsSb = new StringBuilder();
            for (var sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
            {
                var sectionXml =
                    $"<section idx=\"{sectionIndex}\">{Environment.NewLine}{string.Join(Environment.NewLine, sections[sectionIndex].Select(l => l.GetXml()))}{Environment.NewLine}</section>";
                sectionsSb.Append(sectionXml);
                sectionsSb.Append(Environment.NewLine);
            }
            var bodyXml = sectionsSb.ToString();

            // 4. load and fill template
            var template = File.ReadAllText("fb2template.xml");
            var fb2Xml = template.Replace(Tokens.Body, bodyXml).Replace(Tokens.BodyNotes, "");

            // 5. write fb2 file
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
        }
    }


}