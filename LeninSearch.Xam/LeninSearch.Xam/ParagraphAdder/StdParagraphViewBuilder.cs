using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class StdParagraphViewBuilder : IParagraphViewBuilder
    {
        private readonly ILsiProvider _lsiProvider;
        private readonly Action<string> _commentAction;

        public StdParagraphViewBuilder(ILsiProvider lsiProvider, Action<string> commentAction)
        {
            _lsiProvider = lsiProvider;
            _commentAction = commentAction;
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
            //var stackLayout = new StackLayout
            //{
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    HeightRequest = 100
            //};

            //var image = new ZoomImage
            //{
            //    Source = Settings.ImageFile(corpusId, imageIndex),
            //    Margin = new Thickness(5, 5, 5, 5),
            //    HorizontalOptions = LayoutOptions.FillAndExpand
            //};

            //stackLayout.Children.Add(image);

            var imageControl = new ImageControl
            {
                Source = Settings.ImageFile(corpusId, imageIndex)
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
                    var gestureRecognizer = new TapGestureRecognizer { Command = new Command(() =>
                    {
                        var comment = paragraph.Comments.First(c => c.CommentIndex == lsiSpan.CommentIndex);
                        var words = comment.WordIndexes.Select(wi => dictionaryWords[wi]).ToList();
                        var text = TextUtil.GetParagraph(words);
                        _commentAction.Invoke(text);

                    })};
                    span.GestureRecognizers.Add(gestureRecognizer);
                }

                if (lsiSpan.Type != LsiSpanType.Plain && lsiSpan.Type != LsiSpanType.InlineImage)
                {
                    span.Text = $" {span.Text} ";
                }

                formattedString.Spans.Add(span);
            }

            return new ExtendedLabel
            {
                JustifyText = true,
                FormattedText = formattedString,
                Margin = new Thickness(0, 5, 0, 0)
            };
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