using LeninSearch.Standard.Core.Corpus.Lsi;
using Xamarin.Forms;

namespace LeninSearch.Xam.ParagraphAdder
{
    public class PropertyHolderParagraphViewBuilder : IParagraphViewBuilder
    {
        private readonly IParagraphViewBuilder _builder;
        private readonly double _fontSize;
        public PropertyHolderParagraphViewBuilder(IParagraphViewBuilder builder, double fontSize)
        {
            _builder = builder;
            _fontSize = fontSize;
        }

        public View Build(LsiParagraph p, State state, string[] dictionaryWords)
        {
            var view = _builder.Build(p, state, dictionaryWords);

            if (view is FlexLayout flex)
            {
                foreach (var flexChild in flex.Children)
                {
                    if (flexChild is Label label)
                    {
                        label.FontSize = _fontSize;
                    }
                }
            }

            if (view is Label viewLabel)
            {
                viewLabel.FontSize = _fontSize;
                if (viewLabel.FormattedText != null)
                {
                    foreach (var span in viewLabel.FormattedText.Spans)
                    {
                        span.FontSize = _fontSize;
                    }
                }
            }

            return view;
        }
    }
}