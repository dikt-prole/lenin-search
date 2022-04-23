using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                            currentParagraphLineFb2.Words.AddRange(currentLine.Words);
                        }
                    }
                }

                var section = pageParagraphLinesFb2.Concat(pageImageLinesFb2).OrderBy(l => l.TopLeftY).ToList();
                sections.Add(section);
            }

            // 3. construct body xml
            var sectionXmls = sections.Select(s => string.Join(Environment.NewLine, s.Select(l => l.GetXml()))).ToList();
            var bodyXml = string.Join(Environment.NewLine, sectionXmls);

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