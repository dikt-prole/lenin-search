using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Annotations;
using Xamarin.Forms;

namespace LeninSearch.Xam.ListItems
{
    public class ReadListItem : INotifyPropertyChanged
    {
        public string CorpusId { get; set; }
        public string File { get; set; }
        public ushort ParagraphIndex { get; set; }
        public FormattedString FormattedText { get; set; }
        public bool IsImage { get; set; }
        public bool IsText => !IsImage;
        public string ImageFile { get; set; }

        private ushort _imageWidthInitial;
        public ushort ImageWidth => (ushort)(_imageWidthInitial * ImageZoomCoefficient);

        private ushort _imageHeightInitial;
        public ushort ImageHeight => (ushort)(_imageHeightInitial * ImageZoomCoefficient);

        private float _imageZoomCoefficient;
        public float ImageZoomCoefficient
        {
            get => _imageZoomCoefficient;
            set
            {
                if (value == _imageZoomCoefficient) return;
                _imageZoomCoefficient = value;
                OnPropertyChanged(nameof(ImageWidth));
                OnPropertyChanged(nameof(ImageHeight));
            }
        }

        public string ImageTitle { get; set; }

        private string _info;
        public string Info
        {
            get => _info;
            set
            {
                if (value == _info) return;
                _info = value;
                OnPropertyChanged(nameof(Info));
            }
        }

        private bool _isMenuShown;
        public bool IsMenuShown
        {
            get => IsText && _isMenuShown;
            set
            {
                if (value == _isMenuShown) return;
                _isMenuShown = value;
                OnPropertyChanged(nameof(IsMenuShown));
            }
        }

        public ReadListItem Self => this;

        public static ReadListItem Construct(string corpusId, string file, LsiParagraph lsiParagraph, 
            ILsiProvider lsiProvider, SearchUnit searchUnit, Func<ushort> imageWidthFunc, Action<string> alertAction)
        {
            var dictionary = lsiProvider.GetDictionary(corpusId);

            var readListItem = new ReadListItem
            {
                CorpusId = corpusId,
                File = file,
                ParagraphIndex = lsiParagraph.Index,
                IsImage = lsiParagraph.IsImage
            };

            if (readListItem.IsImage)
            {
                readListItem.ImageZoomCoefficient = 1;
                readListItem.ImageFile = Settings.ImageFile(corpusId, lsiParagraph.ImageIndex);
                readListItem._imageWidthInitial = imageWidthFunc();
                var imageCfi = Settings.GetCorpusFileItem(corpusId, $"image{lsiParagraph.ImageIndex}.jpeg");
                readListItem._imageHeightInitial = (ushort)(imageCfi.ImageHeight * readListItem._imageWidthInitial / imageCfi.ImageWidth);
                readListItem.ImageTitle = $"иллюстрация №{lsiParagraph.ImageIndex}";
            }
            else
            {
                readListItem.FormattedText = BuildFormattedText(lsiParagraph.GetSpans(searchUnit), lsiParagraph,
                    dictionary, corpusId, alertAction);
            }


            return readListItem;
        }

        private static FormattedString BuildFormattedText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, LsDictionary dictionary, string corpusId, Action<string> alertAction)
        {
            var commentSpans = new Dictionary<LsiSpan, Span>();
            var searchResultSpans = new Dictionary<LsiSpan, Span>();
            var formattedString = new FormattedString();
            foreach (var lsiSpan in lsiSpans)
            {
                var span = new Span
                {
                    Text = lsiSpan.Type == LsiSpanType.InlineImage
                        ? Settings.ImageFile(corpusId, lsiSpan.ImageIndex)
                        : lsiSpan.GetText(dictionary.Words),
                    TextColor = TextColor(lsiSpan.Type),
                    FontAttributes = paragraph.IsHeading
                        ? FontAttributes.Bold
                        : GetFontAttributes(lsiSpan.Type),
                    //FontFamily = paragraph.IsHeading 
                    //    ? Settings.UI.Font.Bold 
                    //    : FontFamily(lsiSpan.Type),
                    TextDecorations = TextDecorations.None,
                    FontSize = Settings.UI.Font.ReadingFontSize
                };

                if (lsiSpan.Type == LsiSpanType.Comment)
                {
                    commentSpans.Add(lsiSpan, span);
                }

                if (lsiSpan.Type == LsiSpanType.SearchResult)
                {
                    searchResultSpans.Add(lsiSpan, span);
                }

                if (lsiSpan.Type != LsiSpanType.Plain && lsiSpan.Type != LsiSpanType.InlineImage)
                {
                    var beforeSpan = formattedString.Spans.LastOrDefault();
                    var needSpaceBefore = beforeSpan != null && beforeSpan.Text.Last() != ' ';
                    span.Text = needSpaceBefore ? $" {span.Text} " : $"{span.Text} ";
                }

                formattedString.Spans.Add(span);
            }

            foreach (var lsiSpan in commentSpans.Keys)
            {
                var gestureRecognizer = new TapGestureRecognizer { Command = new Command(() =>
                {
                    var comment = paragraph.Comments.First(c => c.CommentIndex == lsiSpan.CommentIndex);
                    var words = comment.WordIndexes.Select(wi => dictionary.Words[wi]).ToList();
                    var commentText = $"[{lsiSpan.CommentIndex + 1}] {TextUtil.GetParagraph(words)}";
                    alertAction(commentText);
                }) };
                commentSpans[lsiSpan].GestureRecognizers.Add(gestureRecognizer);
            }

            return formattedString;
        }

        private static FontAttributes GetFontAttributes(LsiSpanType spanType)
        {
            switch (spanType)
            {
                case LsiSpanType.Strong:
                case LsiSpanType.SearchResult:
                    return FontAttributes.Bold;
                case LsiSpanType.Emphasis:
                    return FontAttributes.Italic;
            }

            return FontAttributes.None;
        }
        private static Color TextColor(LsiSpanType spanType)
        {
            switch (spanType)
            {
                case LsiSpanType.SearchResult:
                case LsiSpanType.Comment:
                    return Settings.UI.MainColor;
                default:
                    return Color.Black;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}