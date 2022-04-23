using System.Collections.Generic;
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
            var ocrBookFolder = input[0];
            var fb2Path = input[1];

            // 1. load ocr data
            var ocrData = OcrData.Load(ocrBookFolder);

            // 2. go through pages and create sections, notes & images
            var sections = new List<List<Fb2Line>>();
            var images = new List<string>();
            var notes = new List<string>();
            foreach (var page in ocrData.Pages)
            {
                var pageParagraphs = new List<Fb2Line>();
                var pageImages = new List<Fb2Line>();
            }

            // 3. load and fill template

            // 4. write fb2 file
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
        }
    }


}