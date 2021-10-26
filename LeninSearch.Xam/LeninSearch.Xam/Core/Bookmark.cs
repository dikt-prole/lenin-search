using System;

namespace LeninSearch.Xam.Core
{
    public class Bookmark
    {
        public Guid Id { get; set; }
        public string CorpusItemName { get; set; }
        public string File { get; set; }
        public string BookName { get; set; }
        public ushort ParagraphIndex { get; set; }
        public string ParagraphText { get; set; }
        public DateTime When { get; set; }

        public string GetText()
        {
            var text = $"{When.ToLocalTime():dd-MM-yyyy} {CorpusItemName} {BookName}";

            //if (!string.IsNullOrEmpty(ParagraphText))
            //{
            //    text = $"{text} {new string(ParagraphText.Take(20).ToArray())}";
            //}

            return text;
        }
    }
}