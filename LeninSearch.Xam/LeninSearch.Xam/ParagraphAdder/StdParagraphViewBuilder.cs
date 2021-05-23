using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Optimized;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class StdParagraphViewBuilder : IParagraphViewBuilder
    {
        public View Build(LsParagraph p, State state)
        {
            var paragraphResult = state.ParagraphResults?[state.CurrentParagraphResultIndex];
            if (paragraphResult.ParagraphIndex != p.Index)
            {
                return new ExtendedLabel
                {
                    Text = p.GetText(LsDictionary.Instance.Words), TextColor = Color.Black, JustifyText = true, Margin = new Thickness(0, 5, 0, 0),
                    TabIndex = p.Index
                };
            }

            var pText = p.GetText(LsDictionary.Instance.Words);
            var chain = paragraphResult.WordIndexChains[0];

            var selection = chain.WordIndexes.Select(wi => LsDictionary.Instance.Words[wi]).ToList();

            var fString = new FormattedString();

            fString.Spans.Add(GetHeadingSpan(state));

            fString.Spans.Add(new Span { Text = Environment.NewLine });

            var spans = GetSpans(pText, selection).ToList();

            foreach (var span in spans) fString.Spans.Add(span);

            var pLabel = new ExtendedLabel { TextColor = Color.Black, JustifyText = true, FormattedText = fString, Margin = new Thickness(0, 5, 0, 0) };
            pLabel.TabIndex = p.Index;
            return pLabel;

        }

        private Span GetHeadingSpan(State state)
        {
            var corpusFileItem = state.GetReadingCorpusFileItem();
            var searchParagraphResult = state.GetCurrentSearchParagraphResult();

            var lsiData = LsIndexDataSource.Get(corpusFileItem.Path);

            var headings = lsiData.LsData.GetHeadingsDownToZero(searchParagraphResult.ParagraphIndex);
            var page = lsiData.LsData.GetClosestPage(searchParagraphResult.ParagraphIndex);

            var spanText = corpusFileItem.Name;
            if (page != null || headings.Any())
            {
                var headingText = headings.Count > 0
                    ? string.Join(" - ", headings.Select(h => h.GetText(LsDictionary.Instance.Words)))
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
                BackgroundColor = Settings.MainColor,
                TextColor = Color.White
            };
        }

        private IEnumerable<Span> GetSpans(string text, List<string> selection)
        {
            var lowerText = text.ToLower();

            var selectionIndexes = selection.Select(s => s.ToLower()).Distinct().ToDictionary(s => s, s => lowerText.IndexOf(s));

            var startIndex = 0;
            foreach (var siKvp in selectionIndexes.OrderBy(si => si.Value))
            {
                if (siKvp.Value < 0) continue;

                if (siKvp.Value > startIndex)
                {
                    var fragment = text.Substring(startIndex, siKvp.Value - startIndex);
                    yield return new Span { Text = fragment };
                    startIndex = siKvp.Value;
                }

                var sFragment = text.Substring(startIndex, siKvp.Key.Length);
                yield return new Span { Text = sFragment, FontAttributes = FontAttributes.Bold };
                startIndex = startIndex + sFragment.Length;
            }

            if (startIndex < text.Length - 1)
            {
                var fragment = text.Substring(startIndex);
                yield return new Span { Text = fragment };
            }
        }
    }
}