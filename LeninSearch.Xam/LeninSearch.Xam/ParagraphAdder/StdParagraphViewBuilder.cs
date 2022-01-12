using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class StdParagraphViewBuilder : IParagraphViewBuilder
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly Func<double> _getEffectiveWidthFunc;
        private readonly IMessage _message = DependencyService.Get<IMessage>();

        public StdParagraphViewBuilder(ILsiProvider lsiProvider, Func<double> getEffectiveWidthFunc)
        {
            _lsiProvider = lsiProvider;
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
                    view = Build_PlainText(lsiSpans, p, dictionaryWords);
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
                EffectiveHeightRequest = effectiveHeight,
                Title = $"илл. {imageIndex + 1}"
            };

            return imageControl;
        }

        private View Build_PlainText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, string[] dictionaryWords)
        {
            return new ExtendedLabel
            {
                Text = lsiSpans[0].GetText(dictionaryWords),
                TextColor = Color.Black,
                JustifyText = true,
                Margin = new Thickness(0, 5, 0, 0),
                FontFamily = paragraph.IsHeading 
                    ? Settings.UI.Font.Bold 
                    : Settings.UI.Font.Regular,
                FontSize = Settings.UI.Font.ReadingFontSize,
                HorizontalOptions = paragraph.IsHeading
                    ? LayoutOptions.Center
                    : LayoutOptions.FillAndExpand
            };
        }

        private View Build_FormattedText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, string[] dictionaryWords, State state)
        {
            var commentSpans = new Dictionary<LsiSpan, Span>();
            var searchResultSpans = new Dictionary<LsiSpan, Span>();
            var formattedString = new FormattedString();
            foreach (var lsiSpan in lsiSpans)
            {
                var span = new Span
                {
                    Text = lsiSpan.Type == LsiSpanType.InlineImage
                        ? Settings.ImageFile(state.CorpusId, lsiSpan.ImageIndex)
                        : lsiSpan.GetText(dictionaryWords),
                    TextColor = TextColor(lsiSpan.Type),
                    FontFamily = paragraph.IsHeading 
                        ? Settings.UI.Font.Bold 
                        : FontFamily(lsiSpan.Type),
                    TextDecorations = Xamarin.Forms.TextDecorations.None,
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
                    span.Text = $" {span.Text} ";
                }

                formattedString.Spans.Add(span);
            }

            var paragraphLabel = new ExtendedLabel
            {
                JustifyText = true,
                FormattedText = formattedString,
                Margin = new Thickness(0, 5, 0, 0),
                HorizontalOptions = paragraph.IsHeading
                    ? LayoutOptions.Center
                    : LayoutOptions.FillAndExpand
            };

            foreach (var lsiSpan in searchResultSpans.Keys)
            {
                var gestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        var lsiData = _lsiProvider.GetLsiData(state.CorpusId, state.ReadingFile);
                        var headings = lsiData.GetHeadingsDownToZero(paragraph.Index);
                        var headingTexts = headings.Select(h => h.GetText(dictionaryWords)).ToList();
                        var hintText = string.Join(" - ", headingTexts);
                        _message.LongAlert(hintText);
                    })
                };
                searchResultSpans[lsiSpan].GestureRecognizers.Add(gestureRecognizer);
            }

            if (!commentSpans.Any()) return paragraphLabel;

            var wrapperStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.White
            };
            wrapperStack.Children.Add(paragraphLabel);
            var commentStack = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.Transparent,
                IsVisible = false,
                Spacing = 0
            };
            wrapperStack.Children.Add(commentStack);

            commentStack.Children.Add(CommentDivider()); // top divider

            var commentTexts = new List<string>();
            foreach (var lsiSpan in commentSpans.Keys)
            {
                var comment = paragraph.Comments.First(c => c.CommentIndex == lsiSpan.CommentIndex);
                var words = comment.WordIndexes.Select(wi => dictionaryWords[wi]).ToList();
                commentTexts.Add($"[{lsiSpan.CommentIndex + 1}] {TextUtil.GetParagraph(words)}");
            }
            var commentSummary = string.Join("\n\n", commentTexts);

            var commentLabel = new ExtendedLabel
            {
                FontFamily = Settings.UI.Font.Regular,
                FontSize = Settings.UI.Font.SmallFontSize,
                TextColor = Color.Black,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = commentSummary,
                JustifyText = true,
                Margin = 0,
                BackgroundColor = Color.Transparent
            };
            commentStack.Children.Add(commentLabel);

            commentStack.Children.Add(CommentDivider()); // bottom divider

            foreach (var lsiSpan in commentSpans.Keys)
            {
                var gestureRecognizer = new TapGestureRecognizer {Command = new Command(() => { commentStack.IsVisible = !commentStack.IsVisible; })};
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

        private StackLayout CommentDivider()
        {
            return new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Settings.UI.MainColor,
                HeightRequest = 1,
                Margin = 0
            };
        }
    }
}