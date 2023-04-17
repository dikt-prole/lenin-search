using System;
using LenLib.Xam.Core;

namespace LenLib.Xam.ListItems
{
    public class BookmarkListItem
    {
        public const int TextLengthMax = 80;
        public Guid BookmarkId { get; set; }
        public string Text { get; set; }
        public string CorpusId { get; set; }
        public string CorpusImage { get; set; }
        public string Date { get; set; }
        public string File { get; set; }
        public ushort ParagraphIndex { get; set; }
        public BookmarkListItem Self => this;

        public static BookmarkListItem FromBookmark(Bookmark bookmark)
        {
            var bookmarkListItem = new BookmarkListItem
            {
                BookmarkId = bookmark.Id,
                CorpusId = bookmark.CorpusItemId,
                CorpusImage = Options.IconFile(bookmark.CorpusItemId),
                Date = bookmark.When.ToString("dd.MM.yyyy"),
                File = bookmark.File,
                ParagraphIndex = bookmark.ParagraphIndex,
                Text = $"{bookmark.BookName} > {bookmark.ParagraphText}"
            };

            if (bookmarkListItem.Text.Length > TextLengthMax)
            {
                bookmarkListItem.Text = bookmarkListItem.Text.Substring(0, TextLengthMax) + " ...";
            }

            return bookmarkListItem;
        }
    }
}