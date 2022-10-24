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
            get => _isMenuShown;
            set
            {
                if (value == _isMenuShown) return;
                _isMenuShown = value;
                OnPropertyChanged(nameof(IsMenuShown));
            }
        }

        public ReadListItem Self => this;

        public static ReadListItem Construct(string corpusId, string file, LsiParagraph lsiParagraph, ILsiProvider lsiProvider, SearchUnit searchUnit)
        {
            var dictionary = lsiProvider.GetDictionary(corpusId);
            return new ReadListItem
            {
                CorpusId = corpusId,
                File = file,
                ParagraphIndex = lsiParagraph.Index,
                IsImage = lsiParagraph.IsImage,
                FormattedText = lsiParagraph.IsImage
                    ? null
                    : BuildFormattedText(lsiParagraph.GetSpans(searchUnit), lsiParagraph, dictionary, corpusId)
            };
        }

        private static FormattedString BuildFormattedText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, LsDictionary dictionary, string corpusId)
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