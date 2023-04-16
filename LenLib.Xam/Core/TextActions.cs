using System.Collections.Generic;
using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Corpus.Lsi;

namespace LenLib.Xam.Core
{
    public class TextActions
    {
        private readonly ShareAction _shareAction;
        private readonly BookmarkAction _bookmarkAction;
        private readonly PlayVideoAction _playVideoAction;

        public TextActions(ShareAction shareAction, BookmarkAction bookmarkAction, PlayVideoAction playVideoAction)
        {
            _shareAction = shareAction;
            _bookmarkAction = bookmarkAction;
            _playVideoAction = playVideoAction;
        }

        public void Share(List<LsiParagraph> paragraphs, CorpusItem ci, CorpusFileItem cfi)
        {
            _shareAction(paragraphs, ci, cfi);
        }
        public void Bookmark(LsiParagraph paragraph, CorpusItem ci, CorpusFileItem cfi)
        {
            _bookmarkAction(paragraph, ci, cfi);
        }

        public void PlayVideo(LsiParagraph paragraph, CorpusItem ci, CorpusFileItem cfi)
        {
            _playVideoAction(paragraph, ci, cfi);
        }
    }

    public delegate void ShareAction(List<LsiParagraph> paragraphs, CorpusItem ci, CorpusFileItem cfi);

    public delegate void BookmarkAction(LsiParagraph paragraph, CorpusItem ci, CorpusFileItem cfi);

    public delegate void PlayVideoAction(LsiParagraph paragraph, CorpusItem ci, CorpusFileItem cfi);
}