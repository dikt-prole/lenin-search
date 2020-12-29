using System;
using System.Collections.Generic;
using System.Linq;
using LeninSearch.Xam.Controls;
using LeninSearch.Xam.Core;
using LeninSearch.Xam.Core.Oprimized;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class StdParagraphViewBuilder : IParagraphViewBuilder
    {
        public View Build(OptimizedParagraph p, State state)
        {
            if (p.Index != state.GetCurrentSearchParagraphResult()?.Index)
            {
                return new ExtendedLabel
                {
                    Text = p.GetText(), TextColor = Color.Black, JustifyText = true, Margin = new Thickness(0, 5, 0, 0),
                    TabIndex = p.Index
                };
            }

            var pText = p.GetText();

            var so = state.SearchOptions;
            var selection = TextUtil.GetOrderedWords(so.AdditionalQuery);
            if (!string.IsNullOrWhiteSpace(so.MainQuery))
            {
                selection.Add(so.MainQuery);
            }

            var fString = new FormattedString();

            fString.Spans.Add(GetHeaderSpan(state));

            fString.Spans.Add(new Span { Text = Environment.NewLine });

            var spans = GetSpans(pText, selection).ToList();

            foreach (var span in spans) fString.Spans.Add(span);

            var pLabel = new ExtendedLabel { TextColor = Color.Black, JustifyText = true, FormattedText = fString, Margin = new Thickness(0, 5, 0, 0) };
            pLabel.TabIndex = p.Index;
            return pLabel;

        }

        private Span GetHeaderSpan(State state)
        {
            var corpusFileItem = state.GetReadingCorpusFileItem();
            var searchParagraphResult = state.GetCurrentSearchParagraphResult();

            var ofd = OptimizedFileData.Get(corpusFileItem.Path);

            var header = ofd.GetParagraphHeader(searchParagraphResult.Index);
            var page = ofd.GetPage(searchParagraphResult.Index);
            var pageHeader = corpusFileItem.Name;
            if (page != null || header != null)
            {
                pageHeader = page == null
                    ? header.GetText()
                    : header == null
                        ? $"{corpusFileItem.Name}, стр. {page}"
                        : $"{corpusFileItem.Name}, стр. {page}, {header.GetText()}";
            }

            return new Span
            {
                Text = $" {pageHeader} ",
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