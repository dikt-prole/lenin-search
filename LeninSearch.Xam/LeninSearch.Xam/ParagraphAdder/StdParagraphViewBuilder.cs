using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core.Optimized;
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

        public View Build(LsParagraph p, State state, string[] dictionaryWords)
        {
            ParagraphSearchResult searchResult = null;
            var searchResults = state.PartialParagraphSearchResult?.FileResults(state.ReadingFile);

            if (searchResults?.Any() == true && state.CurrentParagraphResultIndex >= 0)
            {
                searchResult = searchResults[state.CurrentParagraphResultIndex];
            }

            if (searchResult?.ParagraphIndex != p.Index)
            {
                return new ExtendedLabel
                {
                    Text = p.GetText(dictionaryWords), TextColor = Color.Black, JustifyText = true, Margin = new Thickness(0, 5, 0, 0),
                    TabIndex = p.Index
                };
            }

            var pText = p.GetText(dictionaryWords);
            var chain = searchResult.WordIndexChains[0];

            var selection = chain.WordIndexes.Select(wi => dictionaryWords[wi].ToLower()).ToArray();

            var fString = new FormattedString();

            fString.Spans.Add(GetHeadingSpan(state, dictionaryWords));

            fString.Spans.Add(new Span { Text = Environment.NewLine });

            var spans = GetSpans(pText, selection).ToList();

            foreach (var span in spans) fString.Spans.Add(span);

            var pLabel = new ExtendedLabel { TextColor = Color.Black, JustifyText = true, FormattedText = fString, Margin = new Thickness(0, 5, 0, 0) };
            pLabel.TabIndex = p.Index;
            return pLabel;
        }

        private IEnumerable<Span> GetSpans(string text, string[] selection)
        {
            var lowerText = text.ToLower();

            var selectionIndexes = new List<Tuple<int, string>>();
            foreach (var token in selection)
            {
                var selectionIndex = lowerText.IndexOf(token, 0);
                while (selectionIndex >= 0)
                {
                    selectionIndexes.Add(new Tuple<int, string>(selectionIndex, token));
                    selectionIndex = lowerText.IndexOf(token, selectionIndex + token.Length);
                }
            }

            var startIndex = 0;
            foreach (var si in selectionIndexes.OrderBy(si => si.Item1))
            {
                if (si.Item1 < 0) continue;

                if (si.Item1 > startIndex)
                {
                    var fragment = text.Substring(startIndex, si.Item1 - startIndex);
                    yield return new Span { Text = fragment };
                    startIndex = si.Item1;
                }

                var sFragment = text.Substring(startIndex, si.Item2.Length);
                yield return new Span { Text = sFragment, FontAttributes = FontAttributes.Bold };
                startIndex = startIndex + sFragment.Length;
            }

            if (startIndex < text.Length - 1)
            {
                var fragment = text.Substring(startIndex);
                yield return new Span { Text = fragment };
            }
        }

        private Span GetHeadingSpan(State state, string[] words)
        {
            var corpusItem = state.GetCurrentCorpusItem();
            var corpusFileItem = state.GetReadingCorpusFileItem();
            var searchParagraphResult = state.GetCurrentSearchParagraphResult();

            var lsiData = _lsiProvider.GetLsiData(corpusItem.Id, corpusFileItem.Path);

            var headings = lsiData.LsData.GetHeadingsDownToZero(searchParagraphResult.ParagraphIndex);
            var page = lsiData.LsData.GetClosestPage(searchParagraphResult.ParagraphIndex);

            var spanText = corpusFileItem.Name;
            if (page != null || headings.Any())
            {
                var headingText = headings.Count > 0
                    ? string.Join(" - ", headings.Select(h => h.GetText(words)))
                    : null;

                spanText = page == null
                    ? headingText
                    : string.IsNullOrEmpty(headingText)
                        ? $"{corpusFileItem.Name}, стр. {page}"
                        : $"{corpusFileItem.Name}, стр. {page}, {headingText}";
            }

            return new Span
            {
                Text = spanText,
                BackgroundColor = Settings.UI.MainColor,
                TextColor = Color.White
            };
        }
    }
}