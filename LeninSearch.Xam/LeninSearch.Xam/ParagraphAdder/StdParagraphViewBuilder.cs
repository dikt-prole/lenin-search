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

                // 3. paragraph has inline images
                else if (lsiSpans.Any(s => s.Type == LsiSpanType.InlineImage))
                {
                    view = Build_InlineImages(lsiSpans, dictionaryWords, state.CorpusId);
                }

                // 4. paragraph text is a formatted text without inline images
                else
                {
                    view = Build_FormattedText(lsiSpans, p, dictionaryWords);
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
            return new Image
            {
                Source = Settings.ImageFile(corpusId, imageIndex),
                Margin = new Thickness(5, 5, 5, 5),
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
        }

        private View Build_PlainText(List<LsiSpan> lsiSpans, string[] dictionaryWords)
        {
            return new ExtendedLabel
            {
                Text = lsiSpans[0].GetText(dictionaryWords),
                TextColor = Color.Black,
                JustifyText = true,
                Margin = new Thickness(0, 5, 0, 0)
            };
        }

        private View Build_FormattedText(List<LsiSpan> lsiSpans, LsiParagraph paragraph, string[] dictionaryWords)
        {
            var formattedString = new FormattedString();
            foreach (var lsiSpan in lsiSpans)
            {
                var span = new Span { Text = lsiSpan.GetText(dictionaryWords), TextColor = Color.Black };

                if (lsiSpan.Type == LsiSpanType.SearchResult)
                {
                    span.TextColor = Settings.UI.MainColor;
                    span.FontAttributes = FontAttributes.Bold;
                }

                if (lsiSpan.Type == LsiSpanType.Comment)
                {
                    span.TextColor = Settings.UI.MainColor;
                    var gestureRecognizer = new TapGestureRecognizer { Command = new Command(() =>
                    {
                        var comment = paragraph.Comments.First(c => c.CommentIndex == lsiSpan.CommentIndex);
                        var words = comment.WordIndexes.Select(wi => dictionaryWords[wi]).ToList();
                        var text = TextUtil.GetParagraph(words);
                        _commentAction.Invoke(text);

                    })};
                    span.GestureRecognizers.Add(gestureRecognizer);
                }

                if (lsiSpan.Type == LsiSpanType.Strong)
                {
                    span.FontAttributes = FontAttributes.Bold;
                }

                if (lsiSpan.Type == LsiSpanType.Emphasis)
                {
                    span.FontAttributes = FontAttributes.Italic;
                }

                if (lsiSpan.Type != LsiSpanType.Plain)
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

        private View Build_InlineImages(List<LsiSpan> lsiSpans, string[] dictionaryWords, string corpusId)
        {
            var flexLayout = new FlexLayout
            {
                Direction = FlexDirection.Row,
                AlignItems = FlexAlignItems.End,
                JustifyContent = FlexJustify.SpaceBetween,
                Wrap = FlexWrap.Wrap
            };

            foreach (var lsiSpan in lsiSpans)
            {
                if (lsiSpan.Type == LsiSpanType.InlineImage)
                {
                    flexLayout.Children.Add(new Image
                    {
                        Source = Settings.ImageFile(corpusId, lsiSpan.ImageIndex),
                        WidthRequest = 24,
                        HeightRequest = 24
                    });
                }
                else if (lsiSpan.Type == LsiSpanType.Comment)
                {
                    flexLayout.Children.Add(new Label
                    {
                        Text = lsiSpan.GetText(dictionaryWords),
                        TextColor = Settings.UI.MainColor
                    });
                }
                else if (lsiSpan.Type == LsiSpanType.SearchResult)
                {
                    flexLayout.Children.Add(new Label
                    {
                        Text = lsiSpan.GetText(dictionaryWords),
                        TextColor = Settings.UI.MainColor
                    });
                }
                else
                {
                    foreach (var wi in lsiSpan.WordIndexes)
                    {
                        flexLayout.Children.Add(new Label
                        {
                            Text = dictionaryWords[wi],
                            TextColor = Color.Black,
                            FontAttributes = lsiSpan.Type == LsiSpanType.Strong
                                ? FontAttributes.Bold
                                : lsiSpan.Type == LsiSpanType.Emphasis
                                    ? FontAttributes.Italic
                                    : FontAttributes.None
                        });
                    }
                }
            }

            return flexLayout;
        }
    }
}