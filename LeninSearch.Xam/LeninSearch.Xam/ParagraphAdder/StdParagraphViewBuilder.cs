using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class StdParagraphViewBuilder : IParagraphViewBuilder
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly Action<string> _commentAction;
        private readonly Func<double> _getEffectiveWidthFunc;

        public StdParagraphViewBuilder(ILsiProvider lsiProvider, Action<string> commentAction, Func<double> getEffectiveWidthFunc)
        {
            _lsiProvider = lsiProvider;
            _commentAction = commentAction;
            _getEffectiveWidthFunc = getEffectiveWidthFunc;
        }

        public View Build(LsiParagraph p, State state, string[] dictionaryWords)
        {
            try
            {
                ParagraphSearchResult searchResult = null;
                var searchResults = state.PartialParagraphSearchResult?.FileResults(state.ReadingFile);
                if (searchResults?.Any() == true && state.CurrentParagraphResultIndex >= 0)
                {
                    searchResult = searchResults[state.CurrentParagraphResultIndex];
                }

                View view = null;
                var lsiSpans = p.GetSpans(searchResult);

                // 1. the whole paragraph is an image
                if (p.IsImage)
                {
                    view = Build_Image(state.CorpusId, p.ImageIndex);
                }

                // 2. the whole paragraph is a plain text
                else if (lsiSpans.Count == 1 && lsiSpans[0].Type == LsiSpanType.Plain)
                {
                    view = Build_PlainText(lsiSpans, dictionaryWords);
                }

                // 3. paragraph text is a formatted text (with optional inline images)
                else
                {
                    view = Build_FormattedText(lsiSpans, p, dictionaryWords, state);
                }

                view.TabIndex = p.Index;

                return view;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }
        }

        private View Build_Image(string corpusId, ushort imageIndex)
        {
            var imageFile = Settings.ImageFile(corpusId, imageIndex);
            var effectiveWidth = _getEffectiveWidthFunc();
            var imageCfi = Settings.GetCorpusFileItem(corpusId, $"image{imageIndex}.jpeg");
            var effectiveHeight = imageCfi.ImageHeight * effectiveWidth / imageCfi.ImageWidth;
            var imageControl = new ImageControl
            {
                Source = imageFile,
                EffectiveWidthRequest = effectiveWidth,
                EffectiveHeightRequest = effectiveHeight
            };

            return imageControl;
        }

        private View Build_PlainText(List<LsiSpan> lsiSpans, string[] dictionaryWords)
        {
            return new ExtendedLabel
            {
                Text = lsiSpans[0].GetText(dictionaryWords),
                TextColor = Color.Black,
                JustifyText = true,
                Margin = new Thickness(0, 5, 0, 0),
                FontFamily = Settings.UI.Font.Regular
            };
        }

        private View Build_FormattedText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, string[] dictionaryWords, State state)
        {
            var commentSpans = new Dictionary<LsiSpan, Span>();
            var formattedString = new FormattedString();
            foreach (var lsiSpan in lsiSpans)
            {
                var span = new Span
                {
                    Text = lsiSpan.Type == LsiSpanType.InlineImage
                        ? Settings.ImageFile(state.CorpusId, lsiSpan.ImageIndex)
                        : lsiSpan.GetText(dictionaryWords),
                    TextColor = TextColor(lsiSpan.Type),
                    FontFamily = FontFamily(lsiSpan.Type),
                    TextDecorations = TextDecorations(lsiSpan.Type)
                };

                if (lsiSpan.Type == LsiSpanType.Comment)
                {
                    commentSpans.Add(lsiSpan, span);
                }

                if (lsiSpan.Type != LsiSpanType.Plain && lsiSpan.Type != LsiSpanType.InlineImage)
                {
                    span.Text = $" {span.Text} ";
                }

                formattedString.Spans.Add(span);
            }

            var paragraphLabel = new ExtendedLabel
            {
                JustifyText = true,
                FormattedText = formattedString,
                Margin = new Thickness(0, 5, 0, 0)
            };

            if (!commentSpans.Any()) return paragraphLabel;

            var wrapperStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
            };
            wrapperStack.Children.Add(paragraphLabel);
            var commentStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Settings.UI.MainColor,
                IsVisible = false,
                Spacing = 0
            };
            wrapperStack.Children.Add(commentStack);

            var commentSummary = "";
            foreach (var lsiSpan in commentSpans.Keys)
            {
                var comment = paragraph.Comments.First(c => c.CommentIndex == lsiSpan.CommentIndex);
                var words = comment.WordIndexes.Select(wi => dictionaryWords[wi]).ToList();
                var text = TextUtil.GetParagraph(words);
                commentSummary += $"[{lsiSpan.CommentIndex + 1}] {text}\n\n";
            }
            var commentLabel = new ExtendedLabel
            {
                FontFamily = Settings.UI.Font.Regular,
                FontSize = Settings.UI.Font.SmallFontSize,
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = commentSummary,
                JustifyText = true,
                Margin = new Thickness(10, 10, 0, 0)
            };
            commentStack.Children.Add(commentLabel);
            var commentButtonStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical, 
                WidthRequest = 24,
                HorizontalOptions = LayoutOptions.End,
                Spacing = 0
            };
            commentStack.Children.Add(commentButtonStack);
            var closeButton = new ImageButton
            {
                Source = "close.png",
                WidthRequest = 24,
                HeightRequest = 24,
                Padding = 5,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Settings.UI.MainColor,
                Margin = 0
            };
            closeButton.Clicked += (sender, args) => { commentStack.IsVisible = false; };
            commentButtonStack.Children.Add(closeButton);
            commentStack.Children.Add(new StackLayout{VerticalOptions = LayoutOptions.FillAndExpand});

            foreach (var lsiSpan in commentSpans.Keys)
            {
                var gestureRecognizer = new TapGestureRecognizer {Command = new Command(() => { commentStack.IsVisible = true; })};
                commentSpans[lsiSpan].GestureRecognizers.Add(gestureRecognizer);
            }

            return wrapperStack;
        }
        private string FontFamily(LsiSpanType spanType)
        {
            switch (spanType)
            {
                case LsiSpanType.Plain:
                case LsiSpanType.Comment:
                    return Settings.UI.Font.Regular;
                case LsiSpanType.Strong:
                case LsiSpanType.SearchResult:
                    return Settings.UI.Font.Bold;
                case LsiSpanType.Emphasis:
                    return Settings.UI.Font.Italic;
            }

            return Settings.UI.Font.Regular;
        }

        private Color TextColor(LsiSpanType spanType)
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

        private TextDecorations TextDecorations(LsiSpanType spanType)
        {
            if (spanType == LsiSpanType.SearchResult) return Xamarin.Forms.TextDecorations.Underline;

            return Xamarin.Forms.TextDecorations.None;
        }
    }
}