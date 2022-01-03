using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Xam.Controls;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class StdParagraphViewBuilder : IParagraphViewBuilder
    {
        private readonly ILsiProvider _lsiProvider;

        public StdParagraphViewBuilder(ILsiProvider lsiProvider)
        {
            _lsiProvider = lsiProvider;
        }

        public View Build(LsiParagraph p, State state, string[] dictionaryWords)
        {
            ParagraphSearchResult searchResult = null;
            var searchResults = state.PartialParagraphSearchResult?.FileResults(state.ReadingFile);
            if (searchResults?.Any() == true && state.CurrentParagraphResultIndex >= 0)
            {
                searchResult = searchResults[state.CurrentParagraphResultIndex];
            }

            // 1. the whole paragraph is an image
            if (p.IsImage) return new Image
            {
                Source = Settings.ImageFile(state.CorpusId, p.ImageIndex),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TabIndex = p.Index
            };

            var lsiSpans = p.GetSpans(searchResult);

            // 2. the whole paragraph is a plain text
            if (lsiSpans.Count == 1 && lsiSpans[0].Type == LsiSpanType.Plain)
            {
                return new ExtendedLabel
                {
                    Text = lsiSpans[0].GetText(dictionaryWords),
                    TextColor = Color.Black,
                    JustifyText = true,
                    Margin = new Thickness(0, 5, 0, 0),
                    TabIndex = p.Index
                };
            }

            // 3. paragraph has inline images
            if (lsiSpans.Any(s => s.Type == LsiSpanType.InlineImage))
            {
                var flexLayout = new FlexLayout
                {
                    Direction = FlexDirection.Row,
                    AlignItems = FlexAlignItems.End,
                    JustifyContent = FlexJustify.SpaceBetween,
                    Wrap = FlexWrap.Wrap,
                    TabIndex = p.Index
                };

                foreach (var lsiSpan in lsiSpans)
                {
                    if (lsiSpan.Type == LsiSpanType.InlineImage)
                    {
                        flexLayout.Children.Add(new Image
                        {
                            Source = Settings.ImageFile(state.CorpusId, lsiSpan.ImageIndex),
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

            // 4. paragraph text is a formatted text without inline images
            var formattedString = new FormattedString();
            foreach (var lsiSpan in lsiSpans)
            {
                var span = new Span {Text = lsiSpan.GetText(dictionaryWords), TextColor = Color.Black};

                if (lsiSpan.Type == LsiSpanType.SearchResult || lsiSpan.Type == LsiSpanType.Comment)
                {
                    span.TextColor = Settings.UI.MainColor;
                }

                if (lsiSpan.Type == LsiSpanType.Strong)
                {
                    span.FontAttributes = FontAttributes.Bold;
                }

                if (lsiSpan.Type == LsiSpanType.Emphasis)
                {
                    span.FontAttributes = FontAttributes.Italic;
                }

                formattedString.Spans.Add(span);
            }

            return new ExtendedLabel
            {
                JustifyText = true, 
                FormattedText = formattedString, 
                Margin = new Thickness(0, 5, 0, 0),
                TabIndex = p.Index
            };
        }
    }
}