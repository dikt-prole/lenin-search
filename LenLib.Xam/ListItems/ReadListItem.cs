using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LenLib.Standard.Core;
using LenLib.Standard.Core.Corpus.Lsi;
using LenLib.Standard.Core.Search;
using LenLib.Xam.Core;
using Xamarin.Forms;

namespace LenLib.Xam.ListItems
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

        private FontSize _fontSize;
        public FontSize FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                if (IsText && FormattedText != null)
                {
                    foreach (var span in FormattedText.Spans)
                    {
                        span.FontSize = (int)_fontSize;
                    }
                }
                OnPropertyChanged(nameof(FormattedText));
            }
        }

        public ReadListItem Self => this;

        public static ReadListItem Construct(string corpusId, string file, LsiParagraph lsiParagraph,
            ILsiProvider lsiProvider, SearchUnit searchUnit, Func<ushort> imageWidthFunc, Action<string> alertAction,
            Action<ReadListItem> onReadItemTapped)
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
                readListItem.ImageFile = Options.ImageFile(corpusId, lsiParagraph.ImageIndex);
                readListItem._imageWidthInitial = imageWidthFunc();
                var imageCfi = Options.GetCorpusFileItem(corpusId, $"image{lsiParagraph.ImageIndex}.jpeg");
                readListItem._imageHeightInitial = imageCfi == null
                    ? (ushort)0
                    : (ushort)(imageCfi.ImageHeight * readListItem._imageWidthInitial / imageCfi.ImageWidth);
                readListItem.ImageTitle = $"иллюстрация №{lsiParagraph.ImageIndex}";
            }
            else
            {
                readListItem.FormattedText = BuildFormattedText(lsiParagraph.GetSpans(searchUnit), lsiParagraph,
                    dictionary, corpusId, alertAction, () => onReadItemTapped(readListItem));
            }

            return readListItem;
        }

        private static FormattedString BuildFormattedText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, LsDictionary dictionary, string corpusId,
            Action<string> alertAction, Action onReadItemTapped)
        {
            var formattedString = new FormattedString();
            foreach (var lsiSpan in lsiSpans)
            {
                var span = new Span
                {
                    Text = lsiSpan.Type == LsiSpanType.InlineImage
                        ? Options.ImageFile(corpusId, lsiSpan.ImageIndex)
                        : lsiSpan.GetText(dictionary.Words),
                    TextColor = GetTextColor(lsiSpan.Type),
                    FontAttributes = paragraph.IsHeading
                        ? FontAttributes.Bold
                        : GetFontAttributes(lsiSpan.Type),
                    TextDecorations = GetTextDecorations(lsiSpan.Type)
                };

                if (lsiSpan.Type == LsiSpanType.Comment)
                {
                    var gestureRecognizer = new TapGestureRecognizer
                    {
                        Command = new Command(() =>
                        {
                            var comment = paragraph.Comments.First(c => c.CommentIndex == lsiSpan.CommentIndex);
                            var words = comment.WordIndexes.Select(wi => dictionary.Words[wi]).ToList();
                            var commentText = $"[{lsiSpan.CommentIndex + 1}] {TextUtil.GetParagraph(words)}";
                            alertAction(commentText);
                        })
                    };
                    span.GestureRecognizers.Add(gestureRecognizer);
                }
                else
                {
                    var gestureRecognizer = new TapGestureRecognizer
                    {
                        Command = new Command(onReadItemTapped)
                    };
                    span.GestureRecognizers.Add(gestureRecognizer);
                }

                if (lsiSpan.Type != LsiSpanType.Plain && lsiSpan.Type != LsiSpanType.InlineImage)
                {
                    var beforeSpan = formattedString.Spans.LastOrDefault();
                    var needSpaceBefore = beforeSpan != null && beforeSpan.Text.Last() != ' ';
                    span.Text = needSpaceBefore ? $" {span.Text} " : $"{span.Text} ";
                }

                formattedString.Spans.Add(span);
            }

            return formattedString;
        }

        private static FontAttributes GetFontAttributes(LsiSpanType spanType)
        {
            switch (spanType)
            {
                case LsiSpanType.SearchResult:
                case LsiSpanType.Strong:
                    return FontAttributes.Bold;
                case LsiSpanType.Emphasis:
                    return FontAttributes.Italic;
            }

            return FontAttributes.None;
        }

        private static Color GetTextColor(LsiSpanType spanType)
        {
            switch (spanType)
            {
                case LsiSpanType.SearchResult:
                    return Options.UI.Colors.ReadSearchMatchColor;
                case LsiSpanType.Comment:
                    return Options.UI.Colors.MainColor;
                default:
                    return Color.Black;
            }
        }

        private static TextDecorations GetTextDecorations(LsiSpanType spanType)
        {
            if (spanType == LsiSpanType.SearchResult)
            {
                return TextDecorations.Underline;
            }

            return TextDecorations.None;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}